using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using ProductManagementRazorPages.Hubs;
using Services;

namespace ProductManagementRazorPages.Pages.Products;

public class CreateModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IHubContext<SignalrServer> _hubContext;

    public CreateModel(IProductService productService, ICategoryService categoryService, IHubContext<SignalrServer> hubContext)
    {
        _productService = productService;
        _categoryService = categoryService;
        _hubContext = hubContext;
    }

    [BindProperty]
    public Product Product { get; set; } = new();

    public SelectList CategoryList { get; set; } = null!;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("MemberId") == null)
            return RedirectToPage("/Login");

        CategoryList = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetInt32("MemberId") == null)
            return RedirectToPage("/Login");

        if (!ModelState.IsValid)
        {
            CategoryList = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryName");
            return Page();
        }

        _productService.SaveProduct(Product);
        await _hubContext.Clients.All.SendAsync("LoadAllItems");
        return RedirectToPage("Index");
    }
}
