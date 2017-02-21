using Microsoft.EntityFrameworkCore;

namespace NBitcoin.PaymentServer.Services
{
    public class PaymentsDbContext : DbContext
    {
        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
            : base(options) { }

        public DbSet<Gateway> Gateways { get; set; }

        public DbSet<GatewayKeyIndex> GatewayKeyIndexes { get; set; }

        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        //        public DbSet<BitcoinPaymentTransaction> BitcoinPaymentTransactions { get; set; }

        public DbSet<PaymentRequest> PaymentRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Gateway>().HasKey(x => x.Id);
            modelBuilder.Entity<Gateway>().Property(x => x.GatewayNumber).UseSqlServerIdentityColumn();

            modelBuilder.Entity<GatewayKeyIndex>().HasKey(x => x.GatewayId);
            //modelBuilder.Entity<GatewayKeyIndex>().HasOne<Gateway>(x => x.Gateway)
            //    .WithOne((string)null);

            modelBuilder.Entity<PaymentRequest>().HasKey(x => x.PaymentId);
            modelBuilder.Entity<PaymentRequest>().HasOne<Gateway>(x => x.Gateway)
                .WithMany((string)null);

            modelBuilder.Entity<PaymentDetail>().HasKey(x => x.PaymentId);
            modelBuilder.Entity<PaymentDetail>().HasOne<PaymentRequest>(x => x.PaymentRequest)
                .WithOne((string)null);
        }
    }
}
