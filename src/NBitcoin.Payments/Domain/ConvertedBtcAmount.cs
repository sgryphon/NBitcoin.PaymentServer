using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XyzBitcoin.Domain
{
    public class ConvertedBtcAmount
    {
        internal ConvertedBtcAmount(decimal amountBtc, decimal conversionRate)
        {
            AmountBtc = amountBtc;
            ConversionRate = conversionRate;
        }

        public decimal AmountBtc { get; }

        public decimal ConversionRate { get; }
    }
}
