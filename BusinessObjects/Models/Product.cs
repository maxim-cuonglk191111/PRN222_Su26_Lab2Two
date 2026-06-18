using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

public class Product
{
    public int ProductID { get; set; }

    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(40)]
    [Display(Name = "Product Name")]
    public string ProductName { get; set; } = null!;

    [Required]
    [Display(Name = "Category")]
    public int CategoryID { get; set; }

    [ForeignKey(nameof(CategoryID))]
    public Category? Category { get; set; }

    [Range(0, short.MaxValue, ErrorMessage = "Units in stock must be a non-negative value.")]
    [Display(Name = "Units in Stock")]
    public short? UnitsInStock { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be a positive value.")]
    [Column(TypeName = "money")]
    [Display(Name = "Unit Price")]
    public decimal? UnitPrice { get; set; }
}
