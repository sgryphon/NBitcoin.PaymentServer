using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using XyzBitcoin.Contracts;
using XyzBitcoin.Domain;
using XyzBitcoin.Services;

namespace XyzConsole
{
    public class StoreProcessor
    {
        // Acts as the online store:
        //
        //  * Knows the ExtPubKey for generating transaction addresses
        //  * Generates order (and address)
        //  * Registers address with Bitcoin server (needs to know Bitcoin RPC password)
        // TODO: 
        //  * Check for when payment has been made 
        //

        public const string OrderMasterExtPubKeyWif = "tpubD6NzVbkrYhZ4X8PqzxNhizrbPtoJLvk6C64CiFXH7QMKqyjyNd9wRHBHJFpkKwZxFAA7Z1FZqL8qKdsZVj653k2PhaVp1PkKZTMHiTU3BmS";

        private const string rpcPassword = "fxpFpm8ZLiwJcvYLJgQmPjvVsLF6rQc9Ly9N2EvwqFzS";
        //private const string rpcServerUrl = "http://10.2.1.4:18332";
        private const string rpcServerUrl = "http://13.68.221.246:18332";
        private const string rpcUser = "bitcoinrpc";


        private IBitcoinPaymentRepository _paymentRepository = new BitcoinPaymentListRepository();

        public BitcoinPayment ProcessOrder()
        {
            var bitcoinOptions = Microsoft.Extensions.Options.Options.Create(
                new BitcoinOptions()
                {
                    OrderMasterExtPubKey = OrderMasterExtPubKeyWif,
                    RpcPassword = rpcPassword,
                    RpcServerUrl = rpcServerUrl,
                    RpcUser = rpcUser
                });
            var bitcoinService = new BitcoinService(bitcoinOptions, _paymentRepository);

            var mBtc = 9m;
            var currency = "mBTC"; 
            var amountBtc = mBtc / 1000;

            var order = new Order("Person One", "one@example.org", "Donation", mBtc, currency);
            var convertedAmount = bitcoinService.ConvertAmount(order.Currency, order.Amount);

            Console.WriteLine("Registering payment for order {0} for {1} BTC", order.Id, convertedAmount.AmountBtc);
            var bitcoinPaymentTask = bitcoinService.CreatePayment(order.Id.ToString(), convertedAmount.AmountBtc, currency, convertedAmount.ConversionRate);
            bitcoinPaymentTask.Wait(TimeSpan.FromSeconds(5));
            var bitcoinPayment = bitcoinPaymentTask.Result;
            Console.WriteLine("Registered payment {0} with address {1}", bitcoinPayment.IndexNumber, bitcoinPayment.PaymentAddress);

            return bitcoinPayment;
        }

        public void CheckOrder(string orderReference)
        {
            var bitcoinOptions = Microsoft.Extensions.Options.Options.Create(
                new BitcoinOptions()
                {
                    OrderMasterExtPubKey = OrderMasterExtPubKeyWif,
                    RpcPassword = rpcPassword,
                    RpcServerUrl = rpcServerUrl,
                    RpcUser = rpcUser
                });
            var bitcoinService = new BitcoinService(bitcoinOptions, _paymentRepository);

            Console.WriteLine("Checking order {0}", orderReference);
            var status = bitcoinService.CheckPaymentStatus(orderReference);
            if (status.TotalAmountBtc == 0)
            {
                Console.WriteLine(" No payments received yet");
            }
            else
            {
                if (status.ConfirmationLevel < 0)
                {
                    Console.WriteLine(" Payment incomplete. Received so far: {0}", status.TotalAmountBtc);
                }
                else if (status.ConfirmationLevel < 1)
                {
                    Console.WriteLine(" Payment awaiting confirmation. Total received: {0}", status.TotalAmountBtc);
                }
                else
                {
                    Console.WriteLine(" Payment confirmed with {0} confirmations. Total received: {1}", status.ConfirmationLevel, status.TotalAmountBtc);
                }
            }
        }

        /*
    * Run full node as server (20 GB+ disk)

    * Generate offline master key + address

    * Store master address in web config

    * Database to store order ID, name, email, notes/description, expected amount (conversion rate?); time stamp (timeout for valid conversion)

    => On the fly conversion/equivalent amount in mBTC

    * Use order ID + master address to calculate payment address

    * Wait for confirmation/verification that payment received.  Total received, total confirmed (to address)

    * Offline use order ID + master key to calculate receipt key

    * Transfer from receipt key to main account (fixed address)

         */

    }
}
