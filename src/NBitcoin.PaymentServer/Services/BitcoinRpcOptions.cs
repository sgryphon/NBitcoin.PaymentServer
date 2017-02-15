namespace NBitcoin.PaymentServer.Services
{
    public class BitcoinRpcOptions
    {
        public string PaymentAddressLabelPrefix { get; set; }
        public string RpcServerUrl { get; set; }
        public string RpcUser { get; set; }
        public string RpcPassword { get; set; }
    }
}
