using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using ProductManagementRazorPages.Hubs;
using Services;

namespace ProductManagementRazorPages.Pages.Products;

public class DeleteModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IHubContext<SignalrServer> _hubContext;

    public DeleteModel(IProductService productService, IHubContext<SignalrServer> hubContext)
    {
        _productService = productService;
        _hubContext = hubContext;
    }

    [BindProperty]
    public Product Product { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null)
            return RedirectToPage("/Login");

        Product = await _productService.GetByIdAsync(id) ?? new Product();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserId") == null)
            return RedirectToPage("/Login");

        var product = await _productService.GetByIdAsync(Product.ProductID);
        if (product != null)
        {
            await _productService.DeleteAsync(product.ProductID);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
        }

        return RedirectToPage("Index");
    }
}
