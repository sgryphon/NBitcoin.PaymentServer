using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Reflection;

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

            var applicationVersion = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;

            var eventId = new EventId(1, "GetAbout");
            _logger.LogInformation(eventId, "About controller, version {0}, machine {1}", 
                engineFileVersionInfo.ProductVersion, machineName);

            // Web
            var webAssembly = typeof(AboutController).GetTypeInfo().Assembly;
            var webFileVersionInfo = FileVersionInfo.GetVersionInfo(webAssembly.Location);

            var aboutInfo = new
            {
                ApplicationVersion = applicationVersion,
                AssemblyVersion = engineAssembly.GetName().Version.ToString(),
                FileVersion = engineFileVersionInfo.FileVersion,
                InformationalVersion = engineFileVersionInfo.ProductVersion,
                MachineName = machineName,
                Web = new
                {
                    AssemblyVersion = webAssembly.GetName().Version.ToString(),
                    FileVersion = webFileVersionInfo.FileVersion,
                    InformationalVersion = webFileVersionInfo.ProductVersion,
                }
            };

            return aboutInfo;
        }
    }
}
