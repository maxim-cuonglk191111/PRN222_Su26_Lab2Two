using BusinessObjects.Models;

namespace Services;

public interface ICategoryService
{
    List<Category> GetCategories();
}
