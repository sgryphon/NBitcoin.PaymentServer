using NBitcoin.Payment;
using NBitcoin.PaymentServer.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer
{
    public class PaymentProcessor
    {
        ICurrencyConversionService _currencyConversionService;
        //IOptions<BitcoinOptions> _options;
        IPaymentsRepository _repository;
        IVerificationService _verificationService;

        public PaymentProcessor(IPaymentsRepository repository, 
            ICurrencyConversionService currencyConversionService, 
            IVerificationService verificationService)
        {
            //_options = options;
            _repository = repository;
            _currencyConversionService = currencyConversionService;
            _verificationService = verificationService;
        }

        public ConvertedBtcAmount ConvertAmount(string currency, decimal amount)
        {
            var conversionRate = 0.001m;
            var btcAmount = Math.Round(amount * conversionRate, 3);
            return new ConvertedBtcAmount(btcAmount, conversionRate);
        }

        public PaymentStatus CheckPaymentStatus(string orderReference)
        {
            //var bitcoinPayment = _repository.Query().FirstOrDefault(p => p.OrderReference == orderReference);
            //if (bitcoinPayment == null)
            //{
            //    return new BitcoinPaymentStatus(Guid.Empty, -2, 0m);
            //}
            //var address = BitcoinAddress.Create(bitcoinPayment.PaymentAddress);
            //var confirmationLevel = -1;
            //var totalPaidBtc = 0m;
            //var unspentCoins = _bitcoinClient.ListUnspent(0, maxConfirmationsToCheck, address);
            //// Accumulate the most-confirmed transactions first
            //foreach (var coin in unspentCoins.OrderByDescending(u => u.Confirmations))
            //{
            //    totalPaidBtc += coin.Amount.ToUnit(MoneyUnit.BTC);
            //    // Until have at least enough to cover the order amount
            //    if (confirmationLevel == -1 && totalPaidBtc >= bitcoinPayment.AmountBtc)
            //    {
            //        // Record the confirmation level of the transaction that finalised payment
            //        confirmationLevel = (int)coin.Confirmations;
            //    }
            //    // If total is not reached, then confirmationLevel = -1
            //}
            //var status = new BitcoinPaymentStatus(bitcoinPayment.Id, confirmationLevel, totalPaidBtc);
            //if (status.ConfirmationLevel > -1)
            //{
            //    bitcoinPayment.SetPaymentMade();
            //}
            //return status;
            return null;
        }

        public async Task<PaymentDetails> CreatePayment(string orderReference, decimal amountBtc,
            string originalCurrency = null, decimal? conversionRate = null)
        {
            //// Create payment record
            //var bitcoinPayment = new Payment(orderReference, amountBtc, originalCurrency, conversionRate);
            //await _repository.Add(bitcoinPayment);

            //// Generate the ID of the order
            //var indexNumber = bitcoinPayment.IndexNumber;
            //if (indexNumber == 0)
            //{
            //    throw new Exception("Payment index number not set.");
            //}
            //var derivedOrderKey = _masterPubKey.ExtPubKey.Derive((uint)indexNumber);
            //var paymentAddress = derivedOrderKey.PubKey.GetAddress(_masterPubKey.Network);
            //bitcoinPayment.SetPaymentAddress(paymentAddress.ToString());
            //await _repository.Save();

            //// Register address with bitcoin server
            //_bitcoinClient.ImportAddress(paymentAddress, orderAccountLabel, false);
            //bitcoinPayment.SetRegistered();
            //await _repository.Save();

            //return bitcoinPayment;
            return null;
        }

        public Gateway GetGateway(string gatewayReference)
        {
            Guid gatewayId;
            int gatewayNumber;
            Gateway gateway = null;
            if (Guid.TryParse(gatewayReference, out gatewayId))
            {
                gateway = _repository.Gateways().First(x => x.Id == gatewayId);
            }
            else
            {
                if (int.TryParse(gatewayReference, out gatewayNumber))
                {
                    gateway = _repository.Gateways().First(x => x.GatewayNumber == gatewayNumber);
                }
            }
            return gateway;
        }

        public void GetPaymentDetails(string orderReference)
        {
            //var bitcoinPayment = _repository.Query().First(p => p.OrderReference == orderReference);
            //return bitcoinPayment;
        }

    }
}
