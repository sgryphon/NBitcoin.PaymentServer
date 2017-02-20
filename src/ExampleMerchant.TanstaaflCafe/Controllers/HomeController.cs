using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ExampleMerchant.TanstaaflCafe.Controllers
{
    public class HomeController : Controller
    {
        public string GatewayAddress { get; private set; }

        public HomeController(IOptions<NBitcoinPaymentServerOptions> options)
        {
            GatewayAddress = options.Value.GatewayAddress 
                + (!options.Value.GatewayAddress.EndsWith("/") ? "/" : "");
        }

        public IActionResult Index()
        {
            return View(new {GatewayAddress = GatewayAddress});
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
