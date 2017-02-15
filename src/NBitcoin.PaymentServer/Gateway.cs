using System;

namespace NBitcoin.PaymentServer
{
    public class Gateway
    {
        // NOTE: Empty constructor for Entity Framework
        public Gateway()
        {
        }

        public Gateway(string name, string extPubKey)
        {
            Id = Guid.NewGuid();
            Name = name;
            ExtPubKey = extPubKey;
            IsActive = true;

            // TODO: Should be set by clock services
            Created = DateTimeOffset.UtcNow;
        }

        public DateTimeOffset Created { get; set; }

        public string ExtPubKey { get; set; }

        public int GatewayNumber { get; set; }

        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public string Name { get; set; }
    }
}
