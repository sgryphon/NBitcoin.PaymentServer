using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XyzBitcoin.Domain
{
    public class Order
    {
        // NOTE: Empty constructor for Entity Framework
        public Order()
        {
        }

        public Order(string name, string email, string description, decimal amount, string currency)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Description = description;
            Amount = amount;
            Currency = currency;

            // TODO: Should be set by clock services
            Created = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; set; }

        public DateTimeOffset Created { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public int OrderNumber { get; set; }
    }
}
