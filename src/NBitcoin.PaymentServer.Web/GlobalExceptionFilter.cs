using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Web
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }


        public void OnException(ExceptionContext context)
        {
            _logger.LogError(9100, context.Exception, "Unhandled exception in {0}", context.HttpContext.Request.Path);
        }
    }
}
