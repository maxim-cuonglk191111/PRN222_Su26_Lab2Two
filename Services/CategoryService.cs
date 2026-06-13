using BusinessObjects.Models;
using Repositories;

namespace Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public List<Category> GetCategories() => _categoryRepository.GetCategories();
}
