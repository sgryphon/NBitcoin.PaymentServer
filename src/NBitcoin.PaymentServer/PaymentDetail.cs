using System;

namespace NBitcoin.PaymentServer
{
    public class PaymentDetail
    {
        // NOTE: Empty constructor for Entity Framework
        public PaymentDetail()
        {
        }

        public PaymentDetail(PaymentRequest paymentRequest, 
            int keyIndex, string paymentAddress, decimal amountBtc,
            string originalCurrency, decimal? conversionRate)
        {
            PaymentId = paymentRequest.PaymentId;
            PaymentRequest = paymentRequest;

            KeyIndex = keyIndex;
            PaymentAddress = paymentAddress;
            AmountBtc = amountBtc;
            OriginalCurrency = originalCurrency;
            ConversionRate = conversionRate;
            ProcessingStatus = PaymentProcessingStatus.New;

            // TODO: Should be set by clock services
            Created = DateTimeOffset.UtcNow;
            ProcessingStatusModified = Created;
        }

        public decimal AmountBtc { get; set; }

        public decimal? ConversionRate { get; set; }

        public DateTimeOffset Created { get; set; }

        public int? KeyIndex { get; set; }

        public string OriginalCurrency { get; set; }

        public string PaymentAddress
        {
            get;
            set;
        }

        public Guid PaymentId { get; set; }

        public PaymentRequest PaymentRequest { get; set; }

        public PaymentProcessingStatus ProcessingStatus
        {
            get;
            set;
        }

        public DateTimeOffset ProcessingStatusModified { get; set; }


        public void SetRegistered()
        {
            ProcessingStatus = PaymentProcessingStatus.AddressRegistered;
            ProcessingStatusModified = DateTimeOffset.Now;
        }

        public void SetPaymentMade()
        {
            ProcessingStatus = PaymentProcessingStatus.PaymentMade;
            ProcessingStatusModified = DateTimeOffset.Now;
        }

    }
}
