using Accounting.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Currency> Currencies => Set<Currency>();
    }
}
