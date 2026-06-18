using BusinessObjects.Models;
namespace Repositories;
public interface IProductRepository {
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product p);
    Task UpdateAsync(Product p);
    Task DeleteAsync(int id);
    Task<List<Product>> SearchAsync(string k);
}