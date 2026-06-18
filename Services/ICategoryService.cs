using BusinessObjects.Models;
using Repositories;
namespace Services;
public interface ICategoryService {
    Task<List<Category>> GetAllAsync();
}