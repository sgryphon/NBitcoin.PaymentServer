using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using XyzConsole;

namespace NBitcoin.PaymentServer.Utility
{
    public class Program
    {
        private const string rpcPassword = "fxpFpm8ZLiwJcvYLJgQmPjvVsLF6rQc9Ly9N2EvwqFzS";
        //private const string rpcServerUrl = "http://10.2.1.4:18332";
        private const string rpcServerUrl = "http://13.68.221.246:18332";
        private const string rpcUser = "bitcoinrpc";

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, new Dictionary<string, string> {
                { "-o", "operation" },
                { "-n", "ordernumber" },
                { "--order", "ordernumber" },
                { "-b", "amountbtc" },
                { "-f", "fromsecret" },
                { "--from", "fromsecret" },
                { "-t", "toaddress" },
                { "--to", "toaddress" },
                { "-a", "address" },
                });
            Configuration = builder.Build();

            var operation = Configuration["operation"];
            switch (operation?.ToUpperInvariant())
            {
                case "TEST":
                    Test();
                    break;

                case "COLLECT":
                    Collect();
                    break;

                case "PAY":
                    // Customer
                    //  dotnet XyzConsole.dll -o pay -t "mvzXFqRgnaYFaDr2jNiEPHLRf6ycYmGF5A" -b 0.008
                    Pay();
                    break;

                case "CHECK":
                    Check();
                    break;

                case "TRANSFER":
                    // Collection
                    //  dotnet XyzConsole.dll -o transfer -f "cVKhJZCfusxnNnJ94NBtqXb7ratoPq33Z1qGXTFypEwvaqkCh9RD" -t "mvzXFqRgnaYFaDr2jNiEPHLRf6ycYmGF5A" -b 0.2
                    Transfer();
                    break;

                case "LISTUNSPENT":
                    // Customer
                    //  dotnet XyzConsole.dll -o listunspent -a "mvzXFqRgnaYFaDr2jNiEPHLRf6ycYmGF5A"
                    // Collection
                    //  dotnet XyzConsole.dll -o listunspent -a "mw9imiqy5EiCLnKkVCpJDyAKJAzdTRpWhU"

                    var address = Configuration["address"];
                    if (string.IsNullOrWhiteSpace(address))
                    {
                        Console.WriteLine("Usage: dotnet XyzConsole.dll -o listunspent -a <address>");
                        return;
                    }

                    OutputUnspent(address);
                    break;

                default:
                    Console.WriteLine("Usage: dotnet XyzConsole.dll -o <pay|check|collect|transfer|listunspent>");
#if DEBUG
                    Console.ReadLine();
#endif
                    break;
            }
        }

        public static void Check()
        {
            int orderNumber;
            if (!int.TryParse(Configuration["ordernumber"], out orderNumber))
            {
                Console.WriteLine("Usage: dotnet XyzConsole.dll -o check -n <orderNumber>");
                return;
            }
            var paymentProcessor = new PaymentProcessor();
            paymentProcessor.CheckOrder(orderNumber);

        }

        public static void Collect()
        {
            int orderNumber;
            if (!int.TryParse(Configuration["ordernumber"], out orderNumber))
            {
                Console.WriteLine("Usage: dotnet XyzConsole.dll -o collect -n <orderNumber>");
                return;
            }

            var paymentProcessor = new PaymentProcessor();
            paymentProcessor.CollectOrder(orderNumber);
        }

        public static void Pay()
        {
            var toAddress = Configuration["toaddress"];
            decimal amountBtc;
            if (string.IsNullOrWhiteSpace(toAddress) || !decimal.TryParse(Configuration["amountbtc"], out amountBtc))
            {
                Console.WriteLine("Usage: dotnet XyzConsole.dll -o pay -t <toAddress> -b <amountBtc>");
                return;
            }

            var customerProcessor = new CustomerProcessor();
            customerProcessor.PayToAddress(toAddress, amountBtc);
        }

        public static void Test()
        {
            //GenerateMasterKey();

            //var paymentProcessor = new PaymentProcessor();
            //ListAccounts();

            //paymentProcessor.VerifyExtKey();
            //paymentProcessor.VerifyTransferAddress();

            var storeProcessor = new StoreProcessor();
            var payment1 = storeProcessor.ProcessOrder();
            var payment2 = storeProcessor.ProcessOrder();

            OutputUnspent(payment1.PaymentAddress);
            OutputUnspent(payment2.PaymentAddress);

            var customerProcessor = new CustomerProcessor();
            customerProcessor.PayToAddress(payment1.PaymentAddress, payment1.AmountBtc);

            OutputUnspent(payment1.PaymentAddress);
            OutputUnspent(payment2.PaymentAddress);

            storeProcessor.CheckOrder(payment1.OrderReference);
            storeProcessor.CheckOrder(payment2.OrderReference);

            //paymentProcessor.ConnectToServer();

            //paymentProcessor.TransferPayment(order1);

            //ListAccounts();

            Console.ReadLine();
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
            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));

            var blockCount = client.GetBlockCount();
            Console.WriteLine("Block count: {0}", blockCount);
        }

        public static void ListAccounts()
        {
            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));

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

        private static void OutputUnspent(string addressWif)
        {

            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));

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

        public static void Transfer()
        {
            var fromSecret = Configuration["fromsecret"];
            var toAddress = Configuration["toaddress"];
            decimal amountBtc;
            if (string.IsNullOrWhiteSpace(fromSecret) || string.IsNullOrWhiteSpace(toAddress)
                || !decimal.TryParse(Configuration["amountbtc"], out amountBtc))
            {
                Console.WriteLine("Usage: dotnet XyzConsole.dll -o transfer -f <fromSecret> -t <toAddress> -b <amountBtc>");
                return;
            }


            var fromBitcoinSecret = new BitcoinSecret(fromSecret);
            var toBitcoinAddress = BitcoinAddress.Create(toAddress);

            var amount = new Money(amountBtc, MoneyUnit.BTC);
            var fees = new Money(0.001m, MoneyUnit.BTC);

            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));

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
        }


    }
}
