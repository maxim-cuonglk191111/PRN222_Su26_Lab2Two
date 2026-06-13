using BusinessObjects.Models;

namespace Repositories;

public interface ICategoryRepository
{
    List<Category> GetCategories();
}
