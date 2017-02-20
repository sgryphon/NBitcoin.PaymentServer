using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Web.Api
{
    [Route("api/[controller]")]
    public class AboutController : Controller
    {
        ILogger<AboutController> _logger;

        public AboutController(ILogger<AboutController> logger)
        {
            _logger = logger;
        }

        // GET api/about
        [HttpGet]
        public object Get()
        {
            var engineAssembly = typeof(PaymentProcessor).GetTypeInfo().Assembly;
            var engineFileVersionInfo = FileVersionInfo.GetVersionInfo(engineAssembly.Location);

            var machineName = Environment.MachineName;

            var eventId = new EventId(1001, "GetAbout");
            _logger.LogInformation(eventId, "About controller, version {0}, machine {1}", 
                engineFileVersionInfo.ProductVersion, machineName);

            return new
            {
                InformationalVersion = engineFileVersionInfo.ProductVersion,
                MachineName = machineName
            };
        }
    }
}
