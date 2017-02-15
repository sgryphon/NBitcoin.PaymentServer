using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin.PaymentServer;

namespace NBitcoin.PaymentServer.Web.Api
{
    public class GatewayDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
