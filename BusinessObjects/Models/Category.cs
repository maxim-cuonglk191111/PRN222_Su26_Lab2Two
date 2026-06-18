using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Models;

public class Category
{
    public int CategoryID { get; set; }

    [Required]
    [MaxLength(15)]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
