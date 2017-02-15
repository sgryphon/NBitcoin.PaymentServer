using NBitcoin.PaymentServer.Contracts;
using System;

namespace NBitcoin.PaymentServer.Services
{
    public class DummyCurrencyConversionService : ICurrencyConversionService
    {
        public DummyCurrencyConversionService()
        {
        }

        public ConvertedBtcAmount ConvertAmount(string currency, decimal amount)
        {
            // TODO: Look up conversion rate from somewhere
            var conversionRate = 0.001m;
            var btcAmount = Math.Round(amount * conversionRate, 3);
            return new ConvertedBtcAmount(btcAmount, conversionRate);
        }

    }
}
