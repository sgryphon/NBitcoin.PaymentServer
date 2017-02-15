using NBitcoin.PaymentServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Web.Api
{
    public class StatusDto
    {
        public PaymentStatus Status { get; set; }
    }
}
