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
            decimal conversionRate;

            if (string.Equals(currency, "mBTC", StringComparison.OrdinalIgnoreCase))
            {
                conversionRate = 0.001m;
            }
            else if(string.Equals(currency, "USD", StringComparison.OrdinalIgnoreCase))
            {
                // 201702
                conversionRate = 0.00089m;
            }
            else if (string.Equals(currency, "AUD", StringComparison.OrdinalIgnoreCase))
            {
                // 201702
                conversionRate = 0.00068m;
            }
            else
            {
                conversionRate = 1m;
            }
            var btcAmount = Math.Round(amount * conversionRate, 3);
            return new ConvertedBtcAmount(btcAmount, conversionRate);
        }

    }
}
