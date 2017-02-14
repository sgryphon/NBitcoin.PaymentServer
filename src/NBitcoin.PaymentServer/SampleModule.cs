using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer
{
    public class SampleModule : Nancy.NancyModule
    {
        public SampleModule()
        {
            Get("/", args => "Hello World, it's Nancy on .NET Core");
       }
    }
}
