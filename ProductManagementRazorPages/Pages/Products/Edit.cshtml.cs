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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null)
            return RedirectToPage("/Login");

        Product = await _productService.GetByIdAsync(id) ?? new Product();
        CategoryList = new SelectList(await _categoryService.GetAllAsync(), "CategoryID", "CategoryName");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null)
            return RedirectToPage("/Login");

        if (!ModelState.IsValid)
        {
            CategoryList = new SelectList(await _categoryService.GetAllAsync(), "CategoryID", "CategoryName");
            return Page();
        }

        await _productService.UpdateAsync(Product);
        await _hubContext.Clients.All.SendAsync("LoadAllItems");
        return RedirectToPage("Index");
    }
}
