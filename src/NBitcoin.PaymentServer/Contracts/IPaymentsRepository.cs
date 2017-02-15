using System.Linq;
using System.Threading.Tasks;

namespace NBitcoin.PaymentServer.Contracts
{
    public interface IPaymentsRepository
    {
        Task<PaymentDetail> Add(PaymentDetail entity);

        Task<PaymentRequest> Add(PaymentRequest entity);

        IQueryable<Gateway> Gateways();

        IQueryable<PaymentDetail> PaymentDetails();

        IQueryable<PaymentRequest> PaymentRequests();

        Task Save();
    }
}
