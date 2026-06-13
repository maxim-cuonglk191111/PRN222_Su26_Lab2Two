using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ProductManagementRazorPages.Pages.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;

    public IndexModel(IProductService productService)
    {
        _productService = productService;
    }

    public List<Product> Products { get; set; } = new();

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("MemberId") == null)
            return RedirectToPage("/Login");

        Products = _productService.GetProducts();
        return Page();
    }
}
