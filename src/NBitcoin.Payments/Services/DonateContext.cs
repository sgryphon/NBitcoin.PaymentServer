using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XyzBitcoin.Domain;

namespace XyzBitcoin.Services
{
    public class DonateContext : DbContext
    {
        public DonateContext(DbContextOptions<DonateContext> options)
            : base(options) { }

        public DbSet<BitcoinPayment> BitcoinPayments { get; set; }
//        public DbSet<BitcoinPaymentTransaction> BitcoinPaymentTransactions { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BitcoinPayment>().HasKey(x => x.Id);
            modelBuilder.Entity<BitcoinPayment>().Property(x => x.IndexNumber).UseSqlServerIdentityColumn();

            modelBuilder.Entity<Order>().HasKey(x => x.Id);
            modelBuilder.Entity<Order>().Property(x => x.OrderNumber).UseSqlServerIdentityColumn();
        }
    }
}
