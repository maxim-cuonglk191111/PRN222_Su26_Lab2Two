using BusinessObjects.Models;
using DataAccessObjects;

namespace Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CategoryDAO _categoryDAO;

    public CategoryRepository(CategoryDAO categoryDAO)
    {
        _categoryDAO = categoryDAO;
    }

    public List<Category> GetCategories() => _categoryDAO.GetCategories();
}
