using System.ComponentModel.DataAnnotations;

namespace NBitcoin.PaymentServer.Web.Api
{
    public class SubmitDto
    {
        [Required]
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Memo { get; set; }

        public string Reference { get; set; }
    }
}
