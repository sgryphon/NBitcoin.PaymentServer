using Microsoft.Extensions.Options;
using NBitcoin.PaymentServer.Contracts;
using NBitcoin.RPC;
using System;
using System.Linq;
using System.Net;

namespace NBitcoin.PaymentServer.Services
{
    public class BitcoinRpcVerificationService : IVerificationService
    {
        private const int maxConfirmationsToCheck = 1000000;

        RPCClient _bitcoinClient;
        IOptions<BitcoinRpcOptions> _options;
        string _paymentAddressLabelPrefix;

        public BitcoinRpcVerificationService(IOptions<BitcoinRpcOptions> options)
        {
            _options = options;

            // Bitcoin client holds no state between requestes; it is just a facade for the RPC API
            var credentials = new NetworkCredential(_options.Value.RpcUser, _options.Value.RpcPassword);
            var serverUri = new Uri(_options.Value.RpcServerUrl);
            _bitcoinClient = new RPCClient(credentials, serverUri);

            _paymentAddressLabelPrefix = _options.Value.PaymentAddressLabelPrefix;
        }

        public PaymentStatus CheckPaymentStatus(string paymentAddress, decimal requiredAmountBtc)
        {
            var address = BitcoinAddress.Create(paymentAddress);
            var confirmationLevel = -1;
            var totalPaidBtc = 0m;
            var unspentCoins = _bitcoinClient.ListUnspent(0, maxConfirmationsToCheck, address);
            // Accumulate the most-confirmed transactions first
            foreach (var coin in unspentCoins.OrderByDescending(u => u.Confirmations))
            {
                totalPaidBtc += coin.Amount.ToUnit(MoneyUnit.BTC);
                // Until have at least enough to cover the order amount
                if (confirmationLevel == -1 && totalPaidBtc >= requiredAmountBtc)
                {
                    // Record the confirmation level of the transaction that finalised payment
                    confirmationLevel = (int)coin.Confirmations;
                }
                // If total is not reached, then confirmationLevel = -1
            }
            var status = new PaymentStatus(paymentAddress, requiredAmountBtc, confirmationLevel, totalPaidBtc);
            return status;
        }

        public bool RegisterPaymentAddress(int gatewayNumber, string paymentAddress)
        {
            var label = string.Format("{0}-{1}", _paymentAddressLabelPrefix, gatewayNumber);
            var address = BitcoinAddress.Create(paymentAddress);
            _bitcoinClient.ImportAddress(address, label, false);
            return true;
        }

    }
}
