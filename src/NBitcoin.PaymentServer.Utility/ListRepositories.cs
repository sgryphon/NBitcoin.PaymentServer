using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XyzBitcoin.Contracts;
using XyzBitcoin.Domain;
using XyzBitcoin.Services;

namespace XyzConsole
{
    public class BitcoinPaymentListRepository : IBitcoinPaymentRepository
    {
        int indexNumberCounter = 1000;

        List<BitcoinPayment> list = new List<BitcoinPayment>();

        public Task<BitcoinPayment> Add(BitcoinPayment entity)
        {
            // Set index number on save (as database would)
            entity.IndexNumber = Interlocked.Increment(ref indexNumberCounter);
            list.Add(entity);
            return Task.FromResult(entity);
        }

        public IQueryable<BitcoinPayment> Query()
        {
            return list.AsQueryable();
        }

        public Task Save()
        {
            return Task.CompletedTask;
        }
    }

    public class OrderListRepository : IOrderRepository
    {
        List<Order> list = new List<Order>();

        public Task<Order> Add(Order entity)
        {
            list.Add(entity);
            return Task.FromResult(entity);
        }

        public IQueryable<Order> Query()
        {
            return list.AsQueryable();
        }
    }
}
