using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XyzBitcoin.Domain;

namespace XyzBitcoin.Contracts
{
    public interface IOrderRepository
    {
        Task<Order> Add(Order entity);

        IQueryable<Order> Query();
    }
}
