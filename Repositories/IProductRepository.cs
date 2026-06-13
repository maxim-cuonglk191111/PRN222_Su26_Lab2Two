using BusinessObjects.Models;

namespace Repositories;

public interface IProductRepository
{
    List<Product> GetProducts();
    Product? GetProductById(int id);
    void SaveProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
}
