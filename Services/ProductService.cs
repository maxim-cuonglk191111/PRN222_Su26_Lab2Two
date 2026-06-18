using BusinessObjects.Models;
using Repositories;
namespace Services;
public class ProductService : IProductService {
    private readonly IProductRepository _repo;
    public ProductService(IProductRepository repo) => _repo = repo;
    public Task<List<Product>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Product?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task AddAsync(Product p) => _repo.AddAsync(p);
    public Task UpdateAsync(Product p) => _repo.UpdateAsync(p);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<List<Product>> SearchAsync(string k) => _repo.SearchAsync(k);
}