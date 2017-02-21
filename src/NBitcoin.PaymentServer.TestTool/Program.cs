using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NBitcoin.PaymentServer.TestTool
{
    public class Program
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

        private const string TestRpcPassword = "fxpFpm8ZLiwJcvYLJgQmPjvVsLF6rQc9Ly9N2EvwqFzS";
        //private const string rpcServerUrl = "http://10.2.1.4:18332";
        private const string TestRpcServerUrl = "http://13.68.221.246:18332";
        private const string TestRpcUser = "bitcoinrpc";

        private const decimal PaymentFeeBtc = 0.001m;

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            var defaultConfig = new Dictionary<string,string>
                {
                    { "rpcserver", TestRpcServerUrl },
                    { "rpcuser", TestRpcUser },
                    { "rpcpassword", TestRpcPassword },
                };
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(defaultConfig);
            builder.AddCommandLine(args, new Dictionary<string, string> {
                { "-b", "amountbtc" },
                { "-f", "fromsecret" },
                { "--from", "fromsecret" },
                { "-o", "operation" },
                { "-p", "rpcpassword" },
                { "-s", "rpcserver" },
                { "-t", "toaddress" },
                { "--to", "toaddress" },
                { "-u", "rpcuser" },
                });
            Configuration = builder.Build();

            var operation = Configuration["operation"];
            switch (operation?.ToUpperInvariant())
            {
                case "TEST":
                    Test();
                    break;

                case "PAY":
                    // Customer
                    //  dotnet NBitcoin.PaymentServer.TestTool.dll -o pay -f "cVASqZM4FLSmCgGhTSPSNyMgmDBxR85EWgGXCmRpquWn34WeRv4i" -t "mvzXFqRgnaYFaDr2jNiEPHLRf6ycYmGF5A" -b 0.008
                    // Collection (transfer)
                    //  dotnet NBitcoin.PaymentServer.TestTool.dll -o pay -f "cVKhJZCfusxnNnJ94NBtqXb7ratoPq33Z1qGXTFypEwvaqkCh9RD" -t "mvzXFqRgnaYFaDr2jNiEPHLRf6ycYmGF5A" -b 0.2
                    Pay();
                    break;

                case "LISTUNSPENT":
                    // Customer
                    //  dotnet NBitcoin.PaymentServer.TestTool.dll -o listunspent -t "mvzXFqRgnaYFaDr2jNiEPHLRf6ycYmGF5A"
                    // Collection
                    //  dotnet NBitcoin.PaymentServer.TestTool.dll -o listunspent -t "mw9imiqy5EiCLnKkVCpJDyAKJAzdTRpWhU"
                    ListUnspent();
                    break;

                default:
                    Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.TestTool.dll -o <pay|listunspent>");
                    Console.WriteLine("                [-s <rpcserver>] [-u <rpcuser>] [-p <rpcpassword>]");
#if DEBUG
                    Console.ReadLine();
#endif
                    break;
            }
        }

        public static void Test()
        {
            //GenerateMasterKey();

            //var paymentProcessor = new PaymentProcessor();
            //ListAccounts();

            //paymentProcessor.VerifyExtKey();
            //paymentProcessor.VerifyTransferAddress();

            //var storeProcessor = new StoreProcessor();
            //var payment1 = storeProcessor.ProcessOrder();
            //var payment2 = storeProcessor.ProcessOrder();

            //OutputUnspent(payment1.PaymentAddress);
            //OutputUnspent(payment2.PaymentAddress);

            //var customerProcessor = new CustomerProcessor();
            //customerProcessor.PayToAddress(payment1.PaymentAddress, payment1.AmountBtc);

            //OutputUnspent(payment1.PaymentAddress);
            //OutputUnspent(payment2.PaymentAddress);

            //storeProcessor.CheckOrder(payment1.OrderReference);
            //storeProcessor.CheckOrder(payment2.OrderReference);

            ConnectToServer();

            ////paymentProcessor.TransferPayment(order1);

            ////ListAccounts();

            //Console.ReadLine();
        }

        /*
* Run full node as server (20 GB+ disk)

* Generate offline master key + address

* Store master address in web config

* Database to store order ID, name, email, notes/description, expected amount (conversion rate?); time stamp (timeout for valid conversion)

 => On the fly conversion/equivalent amount in mBTC

* Use order ID + master address to calculate payment address

* Wait for confirmation/verification that payment received.  Total received, total confirmed (to address)

  - Remote api get details of server
  - Register address in server wallet => after registering, then display to user
  - Check balance of address from server 

* Offline use order ID + master key to calculate receipt key

  - Should have addresses in server wallet
  - Check balances / incoming transactions
  - Collect all incoming transactions
  - Sign and pay to transfer account

* Transfer from receipt key to main account (fixed address)

         */

        private static RPCClient CreateRpcClient()
        {
            var rpcServerUrl = Configuration["rpcserver"];
            var rpcUser = Configuration["rpcuser"];
            var rpcPassword = Configuration["rpcpassword"];
            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));
            return client;
        }

        private static void SimpleTest()
        {
            var privateKey = new ExtKey();
            var pubKey = privateKey.Neuter();

            uint orderId = 1001;
            var address = pubKey.Derive(orderId).PubKey.GetAddress(Network.TestNet);
            Console.WriteLine(address);

            var key = privateKey.Derive(orderId).PrivateKey;
            var secret = key.GetBitcoinSecret(Network.TestNet);
            Console.WriteLine(secret);

            Console.ReadLine();
        }

        public static void ConnectToServer()
        {
            var client = CreateRpcClient();

            var blockCount = client.GetBlockCount();
            Console.WriteLine("Block count: {0}", blockCount);
        }

        public static void ListAccounts()
        {
            var client = CreateRpcClient();

            // REQUIRES DEVELOPMENT VERSION OF NBitcoin

            /*
             
            var accounts = client.ListAccounts(1, true);
            Console.WriteLine("Got {0} accounts:", accounts.Count());
            var index = 0;
            foreach (var account in accounts)
            {
                Console.WriteLine("[{2}] Account '{0}': {1}", account.AccountName, account.Amount, index++);
                // NOTE: This returns (creates if necessary) the primary receiving address for an account
                //var address = client.GetAccountAddress(account.AccountName);
                //Console.WriteLine("  Account address: {0}", address);

                // We actually want 'getaddressesbyaccount'
                var addresses = client.GetAddressesByAccount(account.AccountName);
                Console.WriteLine(" Got {0} addresses:", addresses.Count());
                var addressIndex = 0;
                foreach (var address in addresses)
                {
                    Console.WriteLine(" [{1}] Address: {0}", address, addressIndex++);
                    Money received = null;
                    try
                    {
                        received = client.GetReceivedByAddress(address, 0);
                        Console.WriteLine("  Received: {0}", received);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("  Exception: {0}", ex);
                    }
                    var unspentList = client.ListUnspent(0, 100, address);
                    Console.WriteLine("  Got {0} unspent", unspentList.Count());
                    var unspentIndex = 0;
                    foreach (var unspent in unspentList)
                    {
                        Console.WriteLine("  [{3}] Amount {0}, Conf. {1}, Outpoint {2}", unspent.Amount, unspent.Confirmations, unspent.OutPoint, unspentIndex++);
                        Console.WriteLine("   ScriptPubKey: {0}", unspent.ScriptPubKey);
                    }
                }
            }

            */
        }

        // Registered payment 1001 with address mkyq6D6KrENffKycqfHpPkyn93BD25cQHu
        // Registered payment 1002 with address mybDRvJjJ8FBshtKMmxgAz17RFG4HYHXyt

        // Checking order 1001, address 'mkyq6D6KrENffKycqfHpPkyn93BD25cQHu'
        // Checking order 1002, address 'mybDRvJjJ8FBshtKMmxgAz17RFG4HYHXyt'

        private static void ListUnspent()
        {
            var addressWif = Configuration["toaddress"];
            if (string.IsNullOrWhiteSpace(addressWif))
            {
                Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.TestTool.dll -o listunspent -a <address>");
                return;
            }

            var client = CreateRpcClient();

            var bitcoinAddress = BitcoinAddress.Create(addressWif);
            var unspentCoins = client.ListUnspent(0, 1000, bitcoinAddress);
            Console.WriteLine("Addr {0} has {1} unspent:", bitcoinAddress, unspentCoins.Count());
            var unspentIndex = 0;
            foreach (var unspent in unspentCoins)
            {
                Console.WriteLine(" [{3}] Amount {0}, Conf. {1}, Outpoint {2}", unspent.Amount, unspent.Confirmations, unspent.OutPoint, unspentIndex++);
                Console.WriteLine("  ScriptPubKey: {0}", unspent.ScriptPubKey);
            }
        }

        public static void Pay()
        {
            // http://www.codeproject.com/Articles/835098/NBitcoin-Build-Them-All

            var fromSecret = Configuration["fromsecret"];
            var toAddress = Configuration["toaddress"];
            decimal amountBtc;
            if (string.IsNullOrWhiteSpace(fromSecret) || string.IsNullOrWhiteSpace(toAddress)
                || !decimal.TryParse(Configuration["amountbtc"], out amountBtc))
            {
                Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.TestTool.dll -o pay -f <fromSecret> -t <toAddress> -b <amountBtc>");
                return;
            }

            var fromBitcoinSecret = new BitcoinSecret(fromSecret);
            var toBitcoinAddress = BitcoinAddress.Create(toAddress);

            var amount = new Money(amountBtc, MoneyUnit.BTC);
            var fees = new Money(PaymentFeeBtc, MoneyUnit.BTC);

            var client = CreateRpcClient();

            var unspentCoins = client.ListUnspent(0, 100000, fromBitcoinSecret.GetAddress());
            var coins = unspentCoins.Select(u => u.AsCoin());

            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                .AddCoins(coins)
                .AddKeys(fromBitcoinSecret)
                .Send(toBitcoinAddress, amount)
                .SendFees(fees)
                .SetChange(fromBitcoinSecret.GetAddress())
                .BuildTransaction(true);

            txBuilder.Verify(tx);

            Console.WriteLine("Submitting transaction: {0}", tx);

            client.SendRawTransaction(tx);

            Console.WriteLine("Transaction sent");

            //Script script = PayToPubkeyHashTemplate.Instance.GenerateScriptSig(.GenerateOutputScript(to);
        }

    }
}
