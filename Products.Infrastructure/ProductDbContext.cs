using Microsoft.EntityFrameworkCore;
using Products.Domain;

namespace Products.Infrastructure
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure ProductId is unique
            modelBuilder.Entity<Product>()
                //.HasIndex(p => p.ProductId)
                .Property(p => p.ProductId)
                .UseIdentityColumn(seed: 100000, increment: 1);
                //.IsUnique();
        }
    }
}
