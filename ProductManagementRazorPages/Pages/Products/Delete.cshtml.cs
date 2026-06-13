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

    public IActionResult OnGet(int id)
    {
        if (HttpContext.Session.GetInt32("MemberId") == null)
            return RedirectToPage("/Login");

        Product = _productService.GetProductById(id) ?? new Product();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetInt32("MemberId") == null)
            return RedirectToPage("/Login");

        var product = _productService.GetProductById(Product.ProductId);
        if (product != null)
        {
            _productService.DeleteProduct(product);
            await _hubContext.Clients.All.SendAsync("LoadAllItems");
        }

        return RedirectToPage("Index");
    }
}
