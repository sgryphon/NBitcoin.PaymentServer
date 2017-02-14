using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using XyzBitcoin.Contracts;
using XyzBitcoin.Domain;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace XyzBitcoin.Services
{
    public class BitcoinService : IBitcoinService
    {
        private const string orderAccountLabel = "Orders";
        private const int maxConfirmationsToCheck = 1000000;
        //private const string accountLabel = "";
        RPCClient _bitcoinClient;
        BitcoinExtPubKey _masterPubKey;

        IOptions<BitcoinOptions> _options;
        IBitcoinPaymentRepository _repository;

        public BitcoinService(IOptions<BitcoinOptions> options, IBitcoinPaymentRepository repository)
        {
            _options = options;
            _repository = repository;

            _masterPubKey = new BitcoinExtPubKey(_options.Value.OrderMasterExtPubKey);

            // Bitcoin client holds no state between requestes; it is just a facade for the RPC API
            var credentials = new NetworkCredential(_options.Value.RpcUser, _options.Value.RpcPassword);
            var serverUri = new Uri(_options.Value.RpcServerUrl);
            _bitcoinClient = new RPCClient(credentials, serverUri);
        }

        public ConvertedBtcAmount ConvertAmount(string currency, decimal amount)
        {
            var conversionRate = 0.001m;
            var btcAmount = Math.Round(amount * conversionRate, 3);
            return new ConvertedBtcAmount(btcAmount, conversionRate);
        }

        public BitcoinPaymentStatus CheckPaymentStatus(string orderReference)
        {
            var bitcoinPayment = _repository.Query().FirstOrDefault(p => p.OrderReference == orderReference);
            if (bitcoinPayment == null)
            {
                return new BitcoinPaymentStatus(Guid.Empty, -2, 0m);
            }
            var address = BitcoinAddress.Create(bitcoinPayment.PaymentAddress);
            var confirmationLevel = -1;
            var totalPaidBtc = 0m;
            var unspentCoins = _bitcoinClient.ListUnspent(0, maxConfirmationsToCheck, address);
            // Accumulate the most-confirmed transactions first
            foreach (var coin in unspentCoins.OrderByDescending(u => u.Confirmations))
            {
                totalPaidBtc += coin.Amount.ToUnit(MoneyUnit.BTC);
                // Until have at least enough to cover the order amount
                if (confirmationLevel == -1 && totalPaidBtc >= bitcoinPayment.AmountBtc)
                {
                    // Record the confirmation level of the transaction that finalised payment
                    confirmationLevel = (int)coin.Confirmations;
                }
                // If total is not reached, then confirmationLevel = -1
            }
            var status = new BitcoinPaymentStatus(bitcoinPayment.Id, confirmationLevel, totalPaidBtc);
            if (status.ConfirmationLevel > -1)
            {
                bitcoinPayment.SetPaymentMade();
            }
            return status;
        }

        public async Task<BitcoinPayment> CreatePayment(string orderReference, decimal amountBtc,
            string originalCurrency = null, decimal? conversionRate = null)
        {
            // Create payment record
            var bitcoinPayment = new BitcoinPayment(orderReference, amountBtc, originalCurrency, conversionRate);
            await _repository.Add(bitcoinPayment);

            // Generate the ID of the order
            var indexNumber = bitcoinPayment.IndexNumber;
            if (indexNumber == 0)
            {
                throw new Exception("Payment index number not set.");
            }
            var derivedOrderKey = _masterPubKey.ExtPubKey.Derive((uint)indexNumber);
            var paymentAddress = derivedOrderKey.PubKey.GetAddress(_masterPubKey.Network);
            bitcoinPayment.SetPaymentAddress(paymentAddress.ToString());
            await _repository.Save();

            // Register address with bitcoin server
            _bitcoinClient.ImportAddress(paymentAddress, orderAccountLabel, false);
            bitcoinPayment.SetRegistered();
            await _repository.Save();

            return bitcoinPayment;
        }

        public BitcoinPayment GetPaymentDetails(string orderReference)
        {
            var bitcoinPayment = _repository.Query().First(p => p.OrderReference == orderReference);
            return bitcoinPayment;
        }

    }
}
