using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Web.Api
{
    public class SubmitDto
    {
        [Required]
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        [Required]
        public string GatewayId { get; set; }

        public string Memo { get; set; }

        public string Reference { get; set; }
    }
}
