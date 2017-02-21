using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer
{
    public class GatewayKeyIndex
    {
        // NOTE: Empty constructor for Entity Framework
        public GatewayKeyIndex()
        {
        }

        public GatewayKeyIndex(Gateway gateway)
        {
            GatewayId = gateway.Id;
            LastKeyIndex = 0;

            // TODO: Should be set by clock services
            Modified = DateTimeOffset.UtcNow;
        }

        public Guid GatewayId { get; set; }

        public int LastKeyIndex { get; set; }

        public DateTimeOffset Modified { get; set; }
    }
}
