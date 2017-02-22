using Microsoft.Extensions.Logging;
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
        ILogger<PaymentProcessor> _logger;
        IPaymentsRepository _repository;
        IVerificationService _verificationService;

        public PaymentProcessor(
            ILogger<PaymentProcessor> logger,
            IPaymentsRepository repository, 
            ICurrencyConversionService currencyConversionService, 
            IVerificationService verificationService)
        {
            _logger = logger;
            _repository = repository;
            _currencyConversionService = currencyConversionService;
            _verificationService = verificationService;
        }

        public ConvertedBtcAmount ConvertAmount(string currency, decimal amount)
        {
            _logger.LogInformation(ServerEventId.ConversionRequest, "Convert {0} {1} to BTC", currency, amount);

            var convertedBtcAmount = _currencyConversionService.ConvertAmount(currency, amount);
            return convertedBtcAmount;
        }

        public PaymentStatus CheckPaymentStatus(Guid gatewayId, Guid paymentId)
        {
            _logger.LogDebug(ServerEventId.CheckPaymentStatus, "Check payment status, gateway '{0}', payment '{0}'", gatewayId, paymentId);

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
            _logger.LogInformation(ServerEventId.CreatePayment, "Gateway {0}, create payment request for {1} [ref: {2}]", gatewayId, amount, reference);

            // Create payment request
            var gateway = _repository.Gateways().First(x => x.Id == gatewayId);
            var paymentRequest = new PaymentRequest(gateway, amount, currency, reference, memo);
            await _repository.Add(paymentRequest);
            await _repository.Save();
            _logger.LogDebug("Payment request {0} created", paymentRequest.PaymentId);

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
                _logger.LogDebug("Converted {0} {1} to {2} BTC", amount, currency, amountBtc);
            }

            // Generate the ID of the order and the address
            var keyIndex = await _repository.NextKeyIndex(gateway);
            if (keyIndex == 0)
            {
                throw new Exception("Payment index number not set.");
            }
            var masterPubKey = new BitcoinExtPubKey(gateway.ExtPubKey);
            var derivedOrderKey = masterPubKey.ExtPubKey.Derive((uint)keyIndex);
            var paymentAddress = derivedOrderKey.PubKey.GetAddress(masterPubKey.Network);
            var paymentAddressWif = paymentAddress.ToWif();
            _logger.LogDebug("Derived payment address [{0}] '{1}'", keyIndex, paymentAddressWif);

            // Save the details
            var paymentDetail = new PaymentDetail(paymentRequest, 
                keyIndex, paymentAddressWif, 
                amountBtc, originalCurrency, conversionRate);
            await _repository.Add(paymentDetail);
            await _repository.Save();

            // Register address with bitcoin server
            _logger.LogDebug("Registering {0} with verification service", paymentAddressWif);
            var registered = _verificationService.RegisterPaymentAddress(gateway.GatewayNumber, paymentAddressWif);
            _logger.LogDebug("Verification result: {0}", registered);
            if (registered)
            {
                paymentDetail.SetRegistered();
                await _repository.Save();
            }

            return paymentDetail;
        }

        public Gateway GetGateway(string gatewayReference)
        {
            _logger.LogInformation(ServerEventId.GetGateway, "Get gateway '{0}'", gatewayReference);

            Guid gatewayId;
            int gatewayNumber;
            Gateway gateway = null;
            if (Guid.TryParse(gatewayReference, out gatewayId))
            {
                gateway = _repository.Gateways()
                    .Where(x => x.IsActive)
                    .First(x => x.Id == gatewayId);
            }
            else
            {
                if (int.TryParse(gatewayReference, out gatewayNumber))
                {
                    gateway = _repository.Gateways()
                        .Where(x => x.IsActive)
                        .First(x => x.GatewayNumber == gatewayNumber);
                }
            }

            _logger.LogDebug("Gateway '{0}' found", gateway.Name);

            return gateway;
        }

        public PaymentDetail GetPaymentDetail(Guid gatewayId, Guid paymentId)
        {
            _logger.LogInformation(ServerEventId.GetPaymentDetails, "Get payment details, gateway '{0}', payment '{1}'", gatewayId, paymentId);

            var paymentDetail = _repository.PaymentDetails(true)
                .Where(p => p.PaymentRequest.Gateway.Id == gatewayId)
                .Where(p => p.PaymentId == paymentId)
                .First();
            return paymentDetail;
        }

    }
}
