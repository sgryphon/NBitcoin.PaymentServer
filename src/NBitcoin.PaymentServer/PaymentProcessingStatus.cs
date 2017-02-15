namespace NBitcoin.PaymentServer
{
    public enum PaymentProcessingStatus
    {
        None = 0,
        New = 1,
        AddressRegistered = 2,
        PaymentMade = 3,
        PaymentCollected = 4
    }
}
