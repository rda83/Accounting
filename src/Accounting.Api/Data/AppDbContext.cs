using Accounting.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(t => t.FromAccount)
                    .WithMany(a => a.CreditTransactions)
                    .HasForeignKey(t => t.FromAccountId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ToAccount)
                    .WithMany(a => a.DebitTransactions)
                    .HasForeignKey(t => t.ToAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.CreatedAt);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Number)
                .IsUnique();
        }
    }
}
