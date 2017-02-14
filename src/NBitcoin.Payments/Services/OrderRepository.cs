using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XyzBitcoin.Contracts;
using XyzBitcoin.Domain;

namespace XyzBitcoin.Services
{
    public class OrderRepository : IOrderRepository
    {
        DonateContext _context;

        public OrderRepository(DonateContext context)
        {
            _context = context;
        }

        public async Task<Order> Add(Order entity)
        {
            _context.Orders.Add(entity);
            // TODO: Move to UnitOfWork pattern
            var count = await _context.SaveChangesAsync();
            if (count == 0)
            {
                return null;
            }
            return entity;
        }

        public IQueryable<Order> Query()
        {
            return _context.Orders;
        }
    }
}
