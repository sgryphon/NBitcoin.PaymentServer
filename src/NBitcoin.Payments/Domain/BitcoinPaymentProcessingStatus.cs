using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XyzBitcoin.Domain
{
    public enum BitcoinPaymentProcessingStatus
    {
        None = 0,
        New = 1,
        AddressRegistered = 2,
        PaymentMade = 3,
        PaymentCollected = 4
    }
}
