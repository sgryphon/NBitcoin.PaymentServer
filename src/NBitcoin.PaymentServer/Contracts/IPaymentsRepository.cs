using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Contracts
{
    public interface IPaymentsRepository
    {
        Task<PaymentDetail> Add(PaymentDetail entity);

        Task<PaymentRequest> Add(PaymentRequest entity);

        IQueryable<Gateway> Gateways();

        Task<int> NextKeyIndex(Gateway gateway);

        IQueryable<PaymentDetail> PaymentDetails(bool include = false);

        IQueryable<PaymentRequest> PaymentRequests();

        Task Save();
    }
}
