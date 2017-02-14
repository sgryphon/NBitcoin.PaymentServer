using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XyzBitcoin.Contracts;
using XyzBitcoin.Domain;

namespace XyzBitcoin.Services
{
    public class BitcoinPaymentRepository : IBitcoinPaymentRepository
    {
        DonateContext _context;

        public BitcoinPaymentRepository(DonateContext context)
        {
            _context = context;
        }

        public async Task<BitcoinPayment> Add(BitcoinPayment entity)
        {
            _context.BitcoinPayments.Add(entity);
            var count = await _context.SaveChangesAsync();
            if (count == 0)
            {
                return null;
            }
            return entity;
        }

        public IQueryable<BitcoinPayment> Query()
        {
            return _context.BitcoinPayments;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
