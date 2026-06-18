using BusinessObjects.Models;
using Repositories;
namespace Services;
public interface IProductService {
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product p);
    Task UpdateAsync(Product p);
    Task DeleteAsync(int id);
    Task<List<Product>> SearchAsync(string k);
}