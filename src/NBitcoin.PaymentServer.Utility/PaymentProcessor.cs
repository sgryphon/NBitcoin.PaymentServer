using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using XyzBitcoin.Domain;

namespace XyzConsole
{
    public class PaymentProcessor
    {
        // Backend payment processing:
        //
        // * Knows master secret for order address generation
        // * Once an order is paid, gets the relevant transactions / balance
        // * Knows RPC details to do this
        // * And forwards all of the money paid to the main transfer account
        // * Knows the transfer account address to send to
        //
        // Utility function:
        // * Knows the transfer account secret key
        // * Can round trip money back to the customer for testing
        //   (knows the customer address)

        private const string rpcPassword = "fxpFpm8ZLiwJcvYLJgQmPjvVsLF6rQc9Ly9N2EvwqFzS";
        //private const string rpcServer = "10.2.1.4";
        //private const string rpcServer = "13.68.221.246";
        private const string rpcServerUrl = "http://13.68.221.246:18332";
        private const string rpcUser = "bitcoinrpc";

        private const string orderMasterSecretWif = "cT1sAzs3G1shP5ssdm4Y2gaBWAHes1pw2TzhPGcHDgye45ULq2Un";
        private const string transferAddressSecretWif = "cVKhJZCfusxnNnJ94NBtqXb7ratoPq33Z1qGXTFypEwvaqkCh9RD";

        public const string TransferAddressWif = "mw9imiqy5EiCLnKkVCpJDyAKJAzdTRpWhU";

        /*
ExtKey: tprv8ZgxMBicQKsPdfN47Ji7KbCUpsHNBbZBcnTRRjUyh8Yw1VVCkELMEnZR88rK3qy7z5jbdZFPzaAgt5dQFLKrsBpJVsN6SjfER1BQU66wAc8
Secret: cT1sAzs3G1shP5ssdm4Y2gaBWAHes1pw2TzhPGcHDgye45ULq2Un
ExtPubKey: tpubD6NzVbkrYhZ4X8PqzxNhizrbPtoJLvk6C64CiFXH7QMKqyjyNd9wRHBHJFpkKwZxFAA7Z1FZqL8qKdsZVj653k2PhaVp1PkKZTMHiTU3BmS
*/

        ExtKey _extKey;

        public ExtKey ExtKey
        {
            get
            {
                if (_extKey == null)
                {
                    var bitcoinExtPubKey = new BitcoinExtPubKey(StoreProcessor.OrderMasterExtPubKeyWif, Network.TestNet);
                    var bitcoinSecret = new BitcoinSecret(orderMasterSecretWif, Network.TestNet);
                    _extKey = new ExtKey(bitcoinExtPubKey, bitcoinSecret);
                }
                return _extKey;
            }
        }

        public void GenerateDestinationAddress()
        {
            var key = new Key();
            var secret = key.GetWif(Network.TestNet);
            var address = key.PubKey.GetAddress(Network.TestNet);

            Console.WriteLine("Secret: {0}", secret);
            Console.WriteLine("Address: {0}", address);
        }

        public void GenerateMasterKey()
        {
            var masterKey = new ExtKey();

            var bitcoinExtKey = masterKey.GetWif(Network.TestNet);
            //var bitcoinExtKey = new BitcoinExtKey(masterKey, Network.TestNet);
            var bitcoinSecret = masterKey.PrivateKey.GetWif(Network.TestNet);
            //var bitcoinSecret = new BitcoinSecret(masterKey.PrivateKey, Network.TestNet);
            var bitcoinExtPubKey = bitcoinExtKey.Neuter();
            //var bitcoinExtPubKey = new BitcoinExtPubKey(masterKey.Neuter(), Network.TestNet);

            Console.WriteLine("ExtKey: {0}", bitcoinExtKey);
            Console.WriteLine("Secret: {0}", bitcoinSecret);
            Console.WriteLine("ExtPubKey: {0}", bitcoinExtPubKey);
        }

        public void CheckOrder(int orderNumber)
        {
            var bitcoinExtPubKey = new BitcoinExtPubKey(StoreProcessor.OrderMasterExtPubKeyWif, Network.TestNet);
            var address = bitcoinExtPubKey.ExtPubKey.Derive((uint)orderNumber).PubKey.GetAddress(bitcoinExtPubKey.Network);

            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));

            Console.WriteLine("Checking order {0}, address '{1}'", orderNumber, address);
            var unspentCoins = client.ListUnspent(0, 1000, address);
            Console.WriteLine("Addr {0} has {1} unspent:", address, unspentCoins.Count());
            var unspentIndex = 0;
            foreach (var unspent in unspentCoins)
            {
                Console.WriteLine(" [{3}] Amount {0}, Conf. {1}, Outpoint {2}", unspent.Amount, unspent.Confirmations, unspent.OutPoint, unspentIndex++);
                Console.WriteLine("  ScriptPubKey: {0}", unspent.ScriptPubKey);
            }
        }

        public void CollectOrder(int orderNumber)
        {
            var bitcoinExtPubKey = new BitcoinExtPubKey(StoreProcessor.OrderMasterExtPubKeyWif);
            var bitcoinSecret = new BitcoinSecret(orderMasterSecretWif);
            var extKey = new ExtKey(bitcoinExtPubKey, bitcoinSecret);
            var fromSecret = new BitcoinSecret(extKey.Derive((uint)orderNumber).PrivateKey, bitcoinExtPubKey.Network);

            var toAddress = BitcoinAddress.Create(TransferAddressWif);

            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));

            Console.WriteLine("Checking order {0}, address '{1}'", orderNumber, fromSecret.GetAddress());
            var unspentCoins = client.ListUnspent(0, 100000, fromSecret.GetAddress());
            Console.WriteLine("Has {0} unspent:", unspentCoins.Count());
            var unspentIndex = 0;
            foreach (var unspent in unspentCoins)
            {
                Console.WriteLine(" [{1}] Amount {0}", unspent.Amount, unspentIndex++);
            }
            var totalSatoshi = unspentCoins.Sum(u => u.Amount.Satoshi);

            Console.WriteLine("Total {0} unspent satoshi", totalSatoshi);

            if (totalSatoshi > 0)
            {
                var coins = unspentCoins.Select(u => u.AsCoin());

                var fees = new Money(0.001m, MoneyUnit.BTC);
                var amount = (new Money(totalSatoshi)) - fees;
                Console.WriteLine("Transferring {0} (plus {1} fees)", amount, fees);

                var txBuilder = new TransactionBuilder();
                var tx = txBuilder
                    .AddCoins(coins)
                    .AddKeys(fromSecret)
                    .Send(toAddress, amount)
                    .SendFees(fees)
                    .SetChange(fromSecret.GetAddress())
                    .BuildTransaction(true);

                txBuilder.Verify(tx);

                Console.WriteLine("Submitting transaction: {0}", tx);

                client.SendRawTransaction(tx);

                Console.WriteLine("Transaction sent");
            }
        }

        public void VerifyExtKey()
        {
            var extKey = ExtKey;
            var bitcoinExtKey = extKey.GetWif(Network.TestNet);
            Console.WriteLine("ExtKey: {0}", bitcoinExtKey);
        }

        public void VerifyTransferAddress()
        {
            var secret = new BitcoinSecret(transferAddressSecretWif);
            var key = secret.PrivateKey;
            var address = key.PubKey.Hash.GetAddress(secret.Network);
            Console.WriteLine("Transfer Address: {0}", address);
        }


        /*
getaccount
gettransaction
importaddress
listaccounts
listaddressgroupings
listunspent
lockunspent 
         */

    }
}
