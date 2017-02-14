using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace XyzConsole
{
    public class CustomerProcessor
    {
        // Acts as the customer:
        //
        // * Has an address with BTC (initially from faucet)
        // * Knows secret key of address
        // * Makes payments to store addresses
        //
        // Faucet:
        //   https://testnet.manu.backend.hamburg/faucet
        //   tx: dc256db51e0cc2b13bc19a5d71f5cd262bda9b82e2f4c184d54b52e79f28203b
        //   return to: mwCwTceJvYV27KXBc3NJZys6CjsgsoeHmf
        // 
        //   https://testnet.coinfaucet.eu/en/
        //   tx: 7947e75a7d14e1025fc2cb5a5c9deb19d5e21fe6fdfc42c57146f28bb35fb5fa
        //
        //   https://kuttler.eu/en/bitcoin/
        //   manual


        public const string CustomerAddressWif = "mvzXFqRgnaYFaDr2jNiEPHLRf6ycYmGF5A";

        private const string customerAddressSecretWif = "cVASqZM4FLSmCgGhTSPSNyMgmDBxR85EWgGXCmRpquWn34WeRv4i";

        private const string rpcPassword = "fxpFpm8ZLiwJcvYLJgQmPjvVsLF6rQc9Ly9N2EvwqFzS";
        //private const string rpcServerUrl = "http://10.2.1.4:18332";
        private const string rpcServerUrl = "http://13.68.221.246:18332";
        private const string rpcUser = "bitcoinrpc";

        public void PayToAddress(string addressWif, decimal amountBtc)
        {
            // http://www.codeproject.com/Articles/835098/NBitcoin-Build-Them-All
            
            var customerSecret = new BitcoinSecret(customerAddressSecretWif);
            var toAddress = BitcoinAddress.Create(addressWif);
            var amount = new Money(amountBtc, MoneyUnit.BTC);
            var fees = new Money(0.001m, MoneyUnit.BTC);

            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));

            var unspentCoins = client.ListUnspent(0, 100000, customerSecret.GetAddress());
            var coins = unspentCoins.Select(u => u.AsCoin());

            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                .AddCoins(coins)
                .AddKeys(customerSecret)
                .Send(toAddress, amount)
                .SendFees(fees)
                .SetChange(customerSecret.GetAddress())
                .BuildTransaction(true);

            txBuilder.Verify(tx);

            Console.WriteLine("Submitting transaction: {0}", tx);

            client.SendRawTransaction(tx);

            Console.WriteLine("Transaction sent");
            //Script script = PayToPubkeyHashTemplate.Instance.GenerateScriptSig(.GenerateOutputScript(to);
        }
    }
}
