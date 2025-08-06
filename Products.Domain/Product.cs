using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Products.Domain;


public class Product
{
    //[Key]
    //public int Id { get; set; }
    [Key]
    [Required]
    [StringLength(6)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int AvailableStock { get; set; } = 0; // Default stock to 0
}
