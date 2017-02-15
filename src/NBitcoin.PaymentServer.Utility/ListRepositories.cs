using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XyzConsole
{
    //public class BitcoinPaymentListRepository : IBitcoinPaymentRepository
    //{
    //    int indexNumberCounter = 1000;

    //    List<Payment> list = new List<Payment>();

    //    public Task<Payment> Add(Payment entity)
    //    {
    //        // Set index number on save (as database would)
    //        entity.IndexNumber = Interlocked.Increment(ref indexNumberCounter);
    //        list.Add(entity);
    //        return Task.FromResult(entity);
    //    }

    //    public IQueryable<Payment> Query()
    //    {
    //        return list.AsQueryable();
    //    }

    //    public Task Save()
    //    {
    //        return Task.CompletedTask;
    //    }
    //}

    //public class OrderListRepository : IOrderRepository
    //{
    //    List<PaymentRequeset> list = new List<PaymentRequeset>();

    //    public Task<PaymentRequeset> Add(PaymentRequeset entity)
    //    {
    //        list.Add(entity);
    //        return Task.FromResult(entity);
    //    }

    //    public IQueryable<PaymentRequeset> Query()
    //    {
    //        return list.AsQueryable();
    //    }
    //}
}
