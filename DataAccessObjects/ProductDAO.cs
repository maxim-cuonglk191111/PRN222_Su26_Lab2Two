using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class ProductDAO
{
    private readonly MyStoreContext _context;

    public ProductDAO(MyStoreContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
        => await _context.Products.AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.ProductName)
            .ToListAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductID == id);

    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Product>> SearchAsync(string keyword)
        => await _context.Products.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.ProductName.Contains(keyword))
            .OrderBy(p => p.ProductName)
            .ToListAsync();
}
