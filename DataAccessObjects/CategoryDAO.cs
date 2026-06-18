using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class CategoryDAO
{
    private readonly MyStoreContext _context;

    public CategoryDAO(MyStoreContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
        => await _context.Categories.AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
}
