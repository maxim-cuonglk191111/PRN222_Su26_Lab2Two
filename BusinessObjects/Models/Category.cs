using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    public string CategoryName { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
