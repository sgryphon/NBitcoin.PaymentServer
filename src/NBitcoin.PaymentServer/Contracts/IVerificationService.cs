namespace NBitcoin.PaymentServer.Contracts
{
    public interface IVerificationService
    {
        PaymentStatus CheckPaymentStatus(string paymentAddress, decimal expectedAmountBtc);

        bool RegisterPaymentAddress(int gatewayNumber, string paymentAddress);
    }
}
