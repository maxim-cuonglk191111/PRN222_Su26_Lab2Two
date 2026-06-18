using BusinessObjects.Models;
using DataAccessObjects;
namespace Repositories;
public class CategoryRepository : ICategoryRepository {
    private readonly CategoryDAO _dao;
    public CategoryRepository(CategoryDAO dao) => _dao = dao;
    public Task<List<Category>> GetAllAsync() => _dao.GetAllAsync();
}