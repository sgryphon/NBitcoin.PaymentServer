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
            Guid gatewayId,
            [FromBody]SubmitDto dto
            )
        {
            _logger.LogDebug("Create payment submit {0}, {1}, {2}, {3}", gatewayId, dto.Amount, dto.Currency, dto.Reference);

            var paymentDetails = await _paymentProcessor.CreatePayment(gatewayId, dto.Amount, dto.Currency, dto.Reference, dto.Memo);
            Response.StatusCode = 201;
            return new SubmitResponseDto {
                GatewayId = gatewayId,
                PaymentId = paymentDetails.PaymentId
            };
            // TODO: Should this return: new CreatedResult(.. or something???
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
            var paymentDetail = _paymentProcessor.GetPaymentDetail(gatewayId, paymentId);
            return new PaymentDto() {
                PaymentDetail = paymentDetail,
                PaymentRequest = paymentDetail.PaymentRequest
            };
        }

        [HttpGet("{gatewayId}/payment/{paymentId}/status")]
        //[ValidateAntiForgeryToken]
        public StatusDto CheckPaymentStatus(Guid gatewayId, Guid paymentId)
        {
            var status = _paymentProcessor.CheckPaymentStatus(gatewayId, paymentId);
            return new StatusDto() { Status = status };
        }

    }
}
