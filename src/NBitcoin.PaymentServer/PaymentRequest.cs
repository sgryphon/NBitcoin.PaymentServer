using System;

namespace NBitcoin.PaymentServer
{
    public class PaymentRequest
    {
        // NOTE: Empty constructor for Entity Framework
        public PaymentRequest()
        {
        }

        public PaymentRequest(Gateway gateway, decimal amount, string currency, string reference, string memo)
        {
            PaymentId = Guid.NewGuid();
            GatewayId = gateway.Id;
            Gateway = gateway;

            Amount = amount;
            Currency = currency;
            Reference = reference;
            Memo = memo;

            // TODO: Should be set by clock services
            Created = DateTimeOffset.UtcNow;
        }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public DateTimeOffset Created { get; set; }

        public Gateway Gateway { get; set; }

        public Guid GatewayId { get; set; }

        public string Memo { get; set; }

        public Guid PaymentId { get; set; }

        public string Reference { get; set; }
    }
}
