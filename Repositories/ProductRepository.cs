using BusinessObjects.Models;
using DataAccessObjects;
namespace Repositories;
public class ProductRepository : IProductRepository {
    private readonly ProductDAO _dao;
    public ProductRepository(ProductDAO dao) => _dao = dao;
    public Task<List<Product>> GetAllAsync() => _dao.GetAllAsync();
    public Task<Product?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task AddAsync(Product p) => _dao.AddAsync(p);
    public Task UpdateAsync(Product p) => _dao.UpdateAsync(p);
    public Task DeleteAsync(int id) => _dao.DeleteAsync(id);
    public Task<List<Product>> SearchAsync(string k) => _dao.SearchAsync(k);
}