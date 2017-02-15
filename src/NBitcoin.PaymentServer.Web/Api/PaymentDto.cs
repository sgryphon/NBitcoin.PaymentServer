using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin.PaymentServer;

namespace NBitcoin.PaymentServer.Web.Api
{
    public class PaymentDto
    {
        public PaymentRequest PaymentRequest { get; set; }
        public PaymentDetail PaymentDetail { get; set; }
    }
}
