using Microsoft.EntityFrameworkCore;
using NBitcoin.PaymentServer.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Services
{
    public class PaymentsRepository : IPaymentsRepository
    {
        PaymentsDbContext _context;

        public PaymentsRepository(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentDetail> Add(PaymentDetail entity)
        {
            _context.PaymentDetails.Add(entity);
            var count = await _context.SaveChangesAsync();
            if (count == 0)
            {
                return null;
            }
            return entity;
        }

        public async Task<PaymentRequest> Add(PaymentRequest entity)
        {
            _context.PaymentRequests.Add(entity);
            // TODO: Move to UnitOfWork pattern
            var count = await _context.SaveChangesAsync();
            if (count == 0)
            {
                return null;
            }
            return entity;
        }

        public IQueryable<Gateway> Gateways()
        {
            return _context.Gateways;
        }

        public IQueryable<PaymentDetail> PaymentDetails(bool include = false)
        {
            var dbSet = _context.PaymentDetails;
            if (include)
            {
                return _context.PaymentDetails.Include(p => p.PaymentRequest.Gateway);
            }
            else
            {
                return _context.PaymentDetails;
            }
        }

        public IQueryable<PaymentRequest> PaymentRequests()
        {
            return _context.PaymentRequests;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}
