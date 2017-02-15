using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Web.Api
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        IConfigurationRoot _configuration;

        public TestController(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        // GET api/test
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var keys = _configuration.AsEnumerable().Select(kvp => kvp.Key).ToArray();
            return keys;
        }

        // GET api/test/connectionstrings:defaultconnection
        [HttpGet("{key}")]
        public string Get(string key)
        {
            if (key == "4")
            {
                throw new Exception("Test");
            }
            var value = _configuration[key];
            return value;
        }

    }
}
