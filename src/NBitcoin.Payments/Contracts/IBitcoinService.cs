using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XyzBitcoin.Domain;

namespace XyzBitcoin.Contracts
{
    public interface IBitcoinService
    {
        ConvertedBtcAmount ConvertAmount(string currency, decimal amount);

        Task<BitcoinPayment> CreatePayment(string orderReference, decimal amountBtc,
            string originalCurrency = null, decimal? conversionRate = null);

        BitcoinPaymentStatus CheckPaymentStatus(string orderReference);

        BitcoinPayment GetPaymentDetails(string orderReference);
    }

}
