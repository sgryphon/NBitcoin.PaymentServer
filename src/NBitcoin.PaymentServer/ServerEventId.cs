using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer
{
    public static class ServerEventId
    {
        public static EventId ServerStartup = 1001;

        public static EventId GetGateway = 2101;
        public static EventId GetPaymentDetails = 2102;
        public static EventId ConversionRequest = 2103;

        public static EventId CreatePayment = 2201;

        public static EventId UnhandledException = 9001;

        public static EventId CheckPaymentStatus = 32001;
    }
}
