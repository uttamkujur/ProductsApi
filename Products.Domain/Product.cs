using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Products.Domain;


public class Product
{
    [Key]
    [Required]
    [StringLength(6)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int AvailableStock { get; set; } = 0; // Default stock to 0
    public bool IsActive { get; set; } = true; // Default to active

    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.Brand, p.Model })
            .IsUnique(); 
    }
}
