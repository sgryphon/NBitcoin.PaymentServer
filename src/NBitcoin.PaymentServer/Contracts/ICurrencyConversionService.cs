namespace NBitcoin.PaymentServer.Contracts
{
    public interface ICurrencyConversionService
    {
        ConvertedBtcAmount ConvertAmount(string currency, decimal amount);
    }
}
