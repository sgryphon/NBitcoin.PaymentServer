using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NBitcoin.PaymentServer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NBitcoin.PaymentServer.Utility
{
    public class Program
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

        private const string TestRpcPassword = "fxpFpm8ZLiwJcvYLJgQmPjvVsLF6rQc9Ly9N2EvwqFzS";
        //private const string rpcServerUrl = "http://10.2.1.4:18332";
        private const string TestRpcServerUrl = "http://13.68.221.246:18332";
        private const string TestRpcUser = "bitcoinrpc";

        private const decimal PaymentFeeBtc = 0.001m;

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            var defaultConfig = new Dictionary<string, string>
                {
                    { "rpcserver", TestRpcServerUrl },
                    { "rpcuser", TestRpcUser },
                    { "rpcpassword", TestRpcPassword },
                };
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(defaultConfig);
            builder.AddCommandLine(args, new Dictionary<string, string> {
                { "-o", "operation" },
                { "-m", "master" },
                { "-n", "network" },
                { "-x", "secret" },
                { "-i", "keyindex" },
                { "--index", "keyindex" },
                { "-b", "amountbtc" },
                { "-p", "rpcpassword" },
                { "-s", "rpcserver" },
                { "-u", "rpcuser" },
                { "-t", "toaddress" },
                { "--to", "toaddress" },
            });
            Configuration = builder.Build();

            var operation = Configuration["operation"];
            switch (operation?.ToUpperInvariant())
            {
                case "CHECK":
                    // Store
                    //  dotnet NBitcoin.PaymentServer.Utility.dll -o check -m "tpubD6NzVbkrYhZ4X8PqzxNhizrbPtoJLvk6C64CiFXH7QMKqyjyNd9wRHBHJFpkKwZxFAA7Z1FZqL8qKdsZVj653k2PhaVp1PkKZTMHiTU3BmS" -i 5
                    Check();
                    break;

                case "COLLECT":
                    // Store
                    //  dotnet NBitcoin.PaymentServer.Utility.dll -o collect -m "tpubD6NzVbkrYhZ4X8PqzxNhizrbPtoJLvk6C64CiFXH7QMKqyjyNd9wRHBHJFpkKwZxFAA7Z1FZqL8qKdsZVj653k2PhaVp1PkKZTMHiTU3BmS" -i 5 -x "cT1sAzs3G1shP5ssdm4Y2gaBWAHes1pw2TzhPGcHDgye45ULq2Un" -t "mw9imiqy5EiCLnKkVCpJDyAKJAzdTRpWhU"
                    Collect();
                    break;

                case "GENERATE":
                    GenerateMasterKey();
                    break;

                default:
                    Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.Utility.dll -o <check|collect>");
#if DEBUG
                    Console.ReadLine();
#endif
                    break;
            }
        }

        private static RPCClient CreateRpcClient()
        {
            var rpcServerUrl = Configuration["rpcserver"];
            var rpcUser = Configuration["rpcuser"];
            var rpcPassword = Configuration["rpcpassword"];
            var credentials = new NetworkCredential(rpcUser, rpcPassword);
            RPCClient client = new RPCClient(credentials, new Uri(rpcServerUrl));
            return client;
        }


        public static void Check()
        {
            string addressWif;
            decimal amountBtc;

            var hasAmount = decimal.TryParse(Configuration["amountbtc"], out amountBtc);

            var toAddressWif = Configuration["toaddress"];
            if (hasAmount && !string.IsNullOrWhiteSpace(toAddressWif))
            {
                var toAddress = BitcoinAddress.Create(toAddressWif);
                addressWif = toAddress.ToWif();
            }
            else
            {
                var masterExtPubKeyWif = Configuration["master"];
                int keyIndex;
                if (hasAmount 
                    && !string.IsNullOrWhiteSpace(masterExtPubKeyWif)
                    && int.TryParse(Configuration["keyindex"], out keyIndex))
                {
                    var bitcoinExtPubKey = new BitcoinExtPubKey(masterExtPubKeyWif);
                    var address = bitcoinExtPubKey.ExtPubKey.Derive((uint)keyIndex).PubKey.GetAddress(bitcoinExtPubKey.Network);
                    Console.WriteLine("Generated address [{0}] '{1}'", keyIndex, address);
                    addressWif = address.ToWif();
                }
                else
                {
                    Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.Utility.dll -o check -m <masterExtPubKey> -i <keyIndex> -b <amountBtc>");
                    Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.Utility.dll -o check -t <toAddress> -b <amountBtc>");
                    return;
                }
            }

            var rpcOptions = new OptionsWrapper<BitcoinRpcOptions>(
                new BitcoinRpcOptions()
                {
                    ServerUrl = Configuration["rpcserver"],
                    RpcUser = Configuration["rpcuser"],
                    RpcPassword = Configuration["rpcpassword"],
                });
            var logger = (new LoggerFactory()).CreateLogger<BitcoinRpcVerificationService>();
            var verificationService = new BitcoinRpcVerificationService(rpcOptions, logger);

            Console.WriteLine("Checking address {0}", addressWif);
            var paymentStatus = verificationService.CheckPaymentStatus(addressWif, amountBtc);

            Console.WriteLine();
            Console.WriteLine("STATUS");
            Console.WriteLine("Address: {0}", paymentStatus.PaymentAddress);
            Console.WriteLine("Required Amount: {0}", paymentStatus.RequiredAmountBtc);
            Console.WriteLine("Total Amount: {0}", paymentStatus.TotalAmountBtc);
            Console.WriteLine("Confirmation Level: {0}", paymentStatus.ConfirmationLevel);
            Console.WriteLine();

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

        public static void Collect()
        {
            var toAddressWif = Configuration["toaddress"];
            var masterExtPubKeyWif = Configuration["master"];
            var secretWif = Configuration["secret"];
            int keyIndex;
            if (string.IsNullOrWhiteSpace(toAddressWif)
                || string.IsNullOrWhiteSpace(masterExtPubKeyWif)
                || string.IsNullOrWhiteSpace(secretWif)
                || !int.TryParse(Configuration["keyindex"], out keyIndex))
            {
                Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.Utility.dll -o collect -m <masterExtPubKey> -i <keyIndex> -x <secret> -t <toAddress>");
                return;
            }

            var bitcoinExtPubKey = new BitcoinExtPubKey(masterExtPubKeyWif);
            var bitcoinSecret = new BitcoinSecret(secretWif);
            var extKey = new ExtKey(bitcoinExtPubKey, bitcoinSecret);
            var fromSecret = new BitcoinSecret(extKey.Derive((uint)keyIndex).PrivateKey, bitcoinExtPubKey.Network);

            var toAddress = BitcoinAddress.Create(toAddressWif);

            var client = CreateRpcClient();

            Console.WriteLine("Checking address [{0}] '{1}'", keyIndex, fromSecret.GetAddress());
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

                var fees = new Money(PaymentFeeBtc, MoneyUnit.BTC);
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

        public static void GenerateMasterKey()
        {
            var networkArg = Configuration["network"];
            Network network;
            if (string.Equals(networkArg, "main", StringComparison.OrdinalIgnoreCase))
            {
                network = Network.Main;
            }
            else
            {
                network = Network.TestNet;
            }

            Console.WriteLine("Generating master key for {0}", network);

            var masterKey = new ExtKey();

            var bitcoinExtKey = masterKey.GetWif(network);
            //var bitcoinExtKey = new BitcoinExtKey(masterKey, Network.TestNet);
            var bitcoinSecret = masterKey.PrivateKey.GetWif(network);
            //var bitcoinSecret = new BitcoinSecret(masterKey.PrivateKey, Network.TestNet);
            var bitcoinExtPubKey = bitcoinExtKey.Neuter();
            //var bitcoinExtPubKey = new BitcoinExtPubKey(masterKey.Neuter(), Network.TestNet);

            Console.WriteLine("ExtKey: {0}", bitcoinExtKey);
            Console.WriteLine("Secret: {0}", bitcoinSecret);
            Console.WriteLine("ExtPubKey: {0}", bitcoinExtPubKey);
        }
    }
}
