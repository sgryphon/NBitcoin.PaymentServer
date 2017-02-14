using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XyzBitcoin.Domain
{
    public class BitcoinPaymentStatus
    {
        public BitcoinPaymentStatus(Guid paymentId, int confirmationLevel, decimal totalAmountBtc)
        {
            PaymentId = paymentId;
            ConfirmationLevel = confirmationLevel;
            TotalAmountBtc = totalAmountBtc;
        }

        public Guid PaymentId { get; }

        /// <summary>
        /// Gets the confirmation level of the total amount required for the order.
        /// Where payment has been made from multiple transaction, this is number of
        /// confirmations for the least confirmed transaction required to fully pay
        /// for the order.
        /// </summary>
        public int ConfirmationLevel { get; }

        /// <summary>
        /// Gets the total amount of all transactions, including unconfirmed ones,
        /// that have been made to the order payment address.
        /// </summary>
        public decimal TotalAmountBtc { get; }
    }
}
