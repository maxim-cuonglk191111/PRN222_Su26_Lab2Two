using BusinessObjects.Models;
using Repositories;

namespace Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public List<Product> GetProducts() => _productRepository.GetProducts();
    public Product? GetProductById(int id) => _productRepository.GetProductById(id);
    public void SaveProduct(Product product) => _productRepository.SaveProduct(product);
    public void UpdateProduct(Product product) => _productRepository.UpdateProduct(product);
    public void DeleteProduct(Product product) => _productRepository.DeleteProduct(product);
}
