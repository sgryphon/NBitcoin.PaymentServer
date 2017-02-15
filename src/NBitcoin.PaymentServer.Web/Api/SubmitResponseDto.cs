using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Web.Api
{
    public class SubmitResponseDto
    {
        public Guid GatewayId { get; set; }

        public Guid PaymentId { get; set; }
    }
}
