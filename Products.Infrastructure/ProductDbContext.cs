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

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .UseIdentityColumn(seed: 100000, increment: 1);

            modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.Brand, p.Model })
            .IsUnique();
        }
    }
}
