using Microsoft.Extensions.Options;
using NBitcoin.Payment;
using NBitcoin.PaymentServer.Contracts;
using NBitcoin.PaymentServer.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer
{
    public class PaymentProcessor
    {
        ICurrencyConversionService _currencyConversionService;
        IPaymentsRepository _repository;
        IVerificationService _verificationService;

        public PaymentProcessor(
            IPaymentsRepository repository, 
            ICurrencyConversionService currencyConversionService, 
            IVerificationService verificationService)
        {
            _repository = repository;
            _currencyConversionService = currencyConversionService;
            _verificationService = verificationService;
        }

        public ConvertedBtcAmount ConvertAmount(string currency, decimal amount)
        {
            var convertedBtcAmount = _currencyConversionService.ConvertAmount(currency, amount);
            return convertedBtcAmount;
        }

        public PaymentStatus CheckPaymentStatus(Guid gatewayId, Guid paymentId)
        {
            var paymentDetail = _repository.PaymentDetails()
                .Where(p => p.PaymentRequest.Gateway.Id == gatewayId)
                .Where(p => p.PaymentId == paymentId)
                .First();
            var paymentStatus = _verificationService.CheckPaymentStatus(paymentDetail.PaymentAddress, paymentDetail.AmountBtc);
            return paymentStatus;
        }

        public async Task<PaymentDetail> CreatePayment(Guid gatewayId, decimal amount,
            string currency, string reference, string memo)
        {
            // Create payment request
            var gateway = _repository.Gateways().First(x => x.Id == gatewayId);
            var paymentRequest = new PaymentRequest(gateway, amount, currency, reference, memo);
            await _repository.Add(paymentRequest);
            await _repository.Save();

            // Convert the amount to BTC
            string originalCurrency;
            decimal amountBtc;
            decimal? conversionRate;
            if (string.IsNullOrEmpty(currency) || string.Equals(currency, "BTC", StringComparison.OrdinalIgnoreCase))
            {
                originalCurrency = null;
                amountBtc = amount;
                conversionRate = null;
            }
            else
            {
                var convertedBtcAmount = _currencyConversionService.ConvertAmount(currency, amount);
                originalCurrency = currency;
                amountBtc = convertedBtcAmount.AmountBtc;
                conversionRate = convertedBtcAmount.ConversionRate;
            }

            // TODO: Generate the ID of the order and the address
            var keyIndex = 1001;
            if (keyIndex == 0)
            {
                throw new Exception("Payment index number not set.");
            }
            var masterPubKey = new BitcoinExtPubKey(gateway.ExtPubKey);
            var derivedOrderKey = masterPubKey.ExtPubKey.Derive((uint)keyIndex);
            var paymentAddress = derivedOrderKey.PubKey.GetAddress(masterPubKey.Network);

            // Save the details
            var paymentDetail = new PaymentDetail(paymentRequest, 
                keyIndex, paymentAddress.ToWif(), 
                amountBtc, originalCurrency, conversionRate);
            await _repository.Add(paymentDetail);
            await _repository.Save();

            // Register address with bitcoin server
            _verificationService.RegisterPaymentAddress(gateway.GatewayNumber, paymentAddress.ToWif());
            paymentDetail.SetRegistered();
            await _repository.Save();

            return paymentDetail;
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

        public PaymentDetail GetPaymentDetail(Guid gatewayId, Guid paymentId)
        {
            var paymentDetail = _repository.PaymentDetails(true)
                .Where(p => p.PaymentRequest.Gateway.Id == gatewayId)
                .Where(p => p.PaymentId == paymentId)
                .First();
            return paymentDetail;
        }

    }
}
