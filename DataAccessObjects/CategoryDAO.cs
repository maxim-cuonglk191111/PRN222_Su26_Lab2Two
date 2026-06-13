using BusinessObjects;
using BusinessObjects.Models;

namespace DataAccessObjects;

public class CategoryDAO
{
    private readonly MyStoreContext _context;

    public CategoryDAO(MyStoreContext context)
    {
        _context = context;
    }

    public List<Category> GetCategories() => _context.Categories.ToList();
}
