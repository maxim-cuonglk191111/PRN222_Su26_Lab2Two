using BusinessObjects.Models;
namespace Repositories;
public interface ICategoryRepository {
    Task<List<Category>> GetAllAsync();
}