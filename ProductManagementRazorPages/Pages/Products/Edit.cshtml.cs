using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using ProductManagementRazorPages.Hubs;
using Services;

namespace ProductManagementRazorPages.Pages.Products;

public class EditModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IHubContext<SignalrServer> _hubContext;

    public EditModel(IProductService productService, ICategoryService categoryService, IHubContext<SignalrServer> hubContext)
    {
        _productService = productService;
        _categoryService = categoryService;
        _hubContext = hubContext;
    }

    [BindProperty]
    public Product Product { get; set; } = null!;

    public SelectList CategoryList { get; set; } = null!;

    public IActionResult OnGet(int id)
    {
        if (HttpContext.Session.GetInt32("MemberId") == null)
            return RedirectToPage("/Login");

        Product = _productService.GetProductById(id) ?? new Product();
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

        _productService.UpdateProduct(Product);
        await _hubContext.Clients.All.SendAsync("LoadAllItems");
        return RedirectToPage("Index");
    }
}
