using System;

namespace NBitcoin.PaymentServer.Web.Api
{
    public class SubmitResponseDto
    {
        public Guid GatewayId { get; set; }

        public Guid PaymentId { get; set; }
    }
}
