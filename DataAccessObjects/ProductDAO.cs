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

    public List<Product> GetProducts() =>
        _context.Products.Include(p => p.Category).ToList();

    public Product? GetProductById(int id) =>
        _context.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == id);

    public void SaveProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void UpdateProduct(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void DeleteProduct(Product product)
    {
        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}
