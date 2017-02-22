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

        public async Task<int> NextKeyIndex(Gateway gateway)
        {
            int keyIndex = 0;

            var success = false;
            try
            {
                await _context.Database.BeginTransactionAsync();
                var gatewayKeyIndex = await _context.GatewayKeyIndexes.FirstAsync(g => g.GatewayId == gateway.Id);
                keyIndex = ++gatewayKeyIndex.LastKeyIndex;
                await _context.SaveChangesAsync();
                success = true;
            }
            finally
            {
                if (success)
                {
                    _context.Database.CommitTransaction();
                }
                else
                {
                    _context.Database.RollbackTransaction();
                }
            }

            //int x = await _context.Database.SqlQuery<int>("UPDATE [dbo].[GatewayKeyIndexes] SET LastKeyIndex = LastKeyIndex + 1 WHERE GatewayId = @p0; SELECT LastKeyIndex FROM  [dbo].[GatewayKeyIndexes] WHERE GatewayId = @p0;");

            //using (var connection = _context.Database.GetDbConnection())
            //{
            //    connection.Open();
            //    using (var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead))
            //    {
            //        using (var command = connection.CreateCommand())
            //        {
            //            command.CommandText = "UPDATE [dbo].[GatewayKeyIndexes] SET LastKeyIndex = LastKeyIndex + 1 WHERE GatewayId = @p0; SELECT LastKeyIndex FROM [dbo].[GatewayKeyIndexes] WHERE GatewayId = @p0;";
            //            command.Transaction = transaction;
            //            command.Parameters.Add(command.CreateParameter());
            //            command.Parameters[0].ParameterName = "p0";
            //            command.Parameters[0].Value = gateway.Id;
            //            var result = await command.ExecuteScalarAsync();
            //            keyIndex = (int)result;
            //        }
            //        transaction.Commit();
            //    }
            //}

            return keyIndex;
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
