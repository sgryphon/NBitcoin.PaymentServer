namespace NBitcoin.PaymentServer
{
    public class PaymentStatus
    {
        public PaymentStatus(string paymentAddress, decimal requiredAmountBtc, int confirmationLevel, decimal totalAmountBtc)
        {
            PaymentAddress = paymentAddress;
            RequiredAmountBtc = requiredAmountBtc;
            ConfirmationLevel = confirmationLevel;
            TotalAmountBtc = totalAmountBtc;
        }

        /// <summary>
        /// Gets the confirmation level of the total amount required for the order
        /// (RequiredAmountBtc).
        /// Where payment has been made from multiple transaction, this is number of
        /// confirmations for the least confirmed transaction required to fully pay
        /// for the order.
        /// </summary>
        public int ConfirmationLevel { get; }

        /// <summary>
        /// Gets the address the status relates to.
        /// </summary>
        public string PaymentAddress { get; }

        /// <summary>
        /// Gets the expected amount for the payment; this is the amount that the
        /// ConfirmationLevel is for.
        /// </summary>
        public decimal RequiredAmountBtc { get; }

        /// <summary>
        /// Gets the total amount of all transactions, including unconfirmed ones,
        /// that have been made to the order payment address.
        /// </summary>
        public decimal TotalAmountBtc { get; }
    }
}
