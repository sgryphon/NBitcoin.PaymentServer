using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XyzBitcoin.Domain;

namespace XyzBitcoin.Contracts
{
    public interface IBitcoinPaymentRepository
    {
        Task<BitcoinPayment> Add(BitcoinPayment entity);

        IQueryable<BitcoinPayment> Query();

        Task Save();
    }
}
