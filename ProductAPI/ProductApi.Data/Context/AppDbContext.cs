using Microsoft.EntityFrameworkCore;
using ProductApi.Models.Entity;

namespace ProductApi.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Product>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Id = GenerateUniqueId();
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        private int GenerateUniqueId()
        {
            Random random = new Random();
            int newId;
            bool exists;

            do
            {
                newId = random.Next(100000, 999999); // Generate 6-digit ID
                exists = Products.Any(p => p.Id == newId); // Ensure it's unique
            } while (exists);

            return newId;
        }
    }
}
