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
                case "CHECK":
                    Check();
                    break;

                case "COLLECT":
                    Collect();
                    break;

                default:
                    Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.Utility.dll -o <check|collect>");
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
                Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.Utility.dll -o check -n <orderNumber>");
                return;
            }
            //var paymentProcessor = new PaymentProcessor();
            //paymentProcessor.CheckOrder(orderNumber);

        }

        public static void Collect()
        {
            int orderNumber;
            if (!int.TryParse(Configuration["ordernumber"], out orderNumber))
            {
                Console.WriteLine("Usage: dotnet NBitcoin.PaymentServer.Utility.dll -o collect -n <orderNumber>");
                return;
            }

            //var paymentProcessor = new PaymentProcessor();
            //paymentProcessor.CollectOrder(orderNumber);
        }
    }
}
