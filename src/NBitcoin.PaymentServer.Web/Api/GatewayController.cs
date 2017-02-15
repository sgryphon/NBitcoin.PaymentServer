using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NBitcoin.PaymentServer.Web.Api
{
    [Route("api/[controller]")]
    public class GatewayController : Controller
    {
        ILogger<GatewayController> _logger;
        PaymentProcessor _paymentProcessor;
        //IOrderRepository _orderRepository;
        //IBitcoinService _bitcoinService;

        public GatewayController(ILogger<GatewayController> logger, PaymentProcessor paymentProcessor)
        {
            _logger = logger;
            _paymentProcessor = paymentProcessor;
            //_orderRepository = orderRepository;
            //_bitcoinService = bitcoinService;
        }

        // GET: /<controller>/
        public string Index()
        {
            return "NBitcoin.PaymentServer";
        }

        [HttpPost("{gatewayId}/payment")]
        //[ValidateAntiForgeryToken]
        public async Task<SubmitResponseDto> Submit(
            [FromBody]SubmitDto dto
            )
        {
            _logger.LogInformation(1101, "Donation controller submit {0}, {1}, {2}, {3}", dto.GatewayId, dto.Amount, dto.Currency, dto.Reference); 
            //var order = new Order(dto.PaymentName, dto.Email, "Donation", dto.AmountMBtc, currency);
            //await _orderRepository.Add(order);
            //var convertedAmount = _bitcoinService.ConvertAmount(order.Currency, order.Amount);
            //var bitcoinPayment = await _bitcoinService.CreatePayment(order.Id.ToString(), convertedAmount.AmountBtc, currency, convertedAmount.ConversionRate);
            Response.StatusCode = 201;
            return new SubmitResponseDto {
                GatewayId = Guid.Empty,
                PaymentId = Guid.Empty
            };
            // TODO: Should this return: new CreatedResult(.. or something???

            //return new PaymentDto { Order = order, Payment = bitcoinPayment };
            //return View("Payment", new PaymentModel { Order = order, Payment = bitcoinPayment });
        }

        [HttpGet("{gatewayReference}")]
        //[ValidateAntiForgeryToken]
        public GatewayDto GetGateway(string gatewayReference)
        {
            var gateway = _paymentProcessor.GetGateway(gatewayReference);
            return new GatewayDto()
            {
                Id = gateway.Id,
                Name = gateway.Name
            };
        }

        [HttpGet("{gatewayId}/payment/{paymentId}")]
        //[ValidateAntiForgeryToken]
        public PaymentDto GetPayment(Guid gatewayId, Guid paymentId)
        {
            //var order = _orderRepository.Query().First(x => x.Id == orderId);
            //var bitcoinPayment = _bitcoinService.GetPaymentDetails(orderId.ToString());
            //return new PaymentDto { Order = order, Payment = bitcoinPayment };
            return new PaymentDto();
        }

        [HttpGet("{gatewayId}/payment/{paymentId}/status")]
        //[ValidateAntiForgeryToken]
        public StatusDto CheckPaymentStatus(Guid gatewayId, Guid paymentId)
        {
            //var status = _bitcoinService.CheckPaymentStatus(orderId.ToString());
            //return new StatusDto { Status = status };
            //return View("Status", new { Status = status });
            return new StatusDto();
        }

    }
}
