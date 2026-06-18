using BusinessObjects.Models;
using Repositories;
namespace Services;
public class CategoryService : ICategoryService {
    private readonly ICategoryRepository _repo;
    public CategoryService(ICategoryRepository repo) => _repo = repo;
    public Task<List<Category>> GetAllAsync() => _repo.GetAllAsync();
}