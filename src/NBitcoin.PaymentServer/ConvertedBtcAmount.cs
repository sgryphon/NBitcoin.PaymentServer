namespace NBitcoin.PaymentServer
{
    public class ConvertedBtcAmount
    {
        internal ConvertedBtcAmount(decimal amountBtc, decimal conversionRate)
        {
            AmountBtc = amountBtc;
            ConversionRate = conversionRate;
        }

        public decimal AmountBtc { get; }

        public decimal ConversionRate { get; }
    }
}
