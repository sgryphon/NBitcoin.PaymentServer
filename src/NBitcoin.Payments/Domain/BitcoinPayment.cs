using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XyzBitcoin.Domain
{
    public class BitcoinPayment
    {
        // NOTE: Empty constructor for Entity Framework
        public BitcoinPayment()
        {
        }

        public BitcoinPayment(string orderReference, decimal amountBtc,
            string originalCurrency, decimal? conversionRate)
        {
            OrderReference = orderReference;
            AmountBtc = amountBtc;
            OriginalCurrency = originalCurrency;
            ConversionRate = conversionRate;
            ProcessingStatus = BitcoinPaymentProcessingStatus.New;

            // TODO: Should be set by clock services
            Created = DateTimeOffset.UtcNow;
            ProcessingStatusModified = Created;
        }

        public Guid Id { get; set; }

        public DateTimeOffset Created { get; set; }

        public decimal AmountBtc { get; set; }

        public decimal? ConversionRate { get; set; }

        public int IndexNumber { get; set; }

        public string OrderReference { get; set; }

        public string OriginalCurrency { get; set; }

        public string PaymentAddress
        {
            get;
            set;
        }

        public BitcoinPaymentProcessingStatus ProcessingStatus
        {
            get;
            set;
        }

        public DateTimeOffset ProcessingStatusModified { get; set; }

        public void SetPaymentAddress(string address)
        {
            PaymentAddress = address;
        }

        public void SetRegistered()
        {
            ProcessingStatus = BitcoinPaymentProcessingStatus.AddressRegistered;
            ProcessingStatusModified = DateTimeOffset.Now;
        }

        public void SetPaymentMade()
        {
            ProcessingStatus = BitcoinPaymentProcessingStatus.PaymentMade;
            ProcessingStatusModified = DateTimeOffset.Now;
        }

    }
}
