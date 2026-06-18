using BusinessObjects;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public class AccountDAO
{
    private readonly MyStoreContext _context;

    public AccountDAO(MyStoreContext context)
    {
        _context = context;
    }

    public async Task<AccountMember?> GetAccountByEmailAsync(string email)
        => await _context.AccountMembers.AsNoTracking()
            .FirstOrDefaultAsync(a => a.EmailAddress == email);

    public async Task<AccountMember?> GetAccountByIdAsync(string id)
        => await _context.AccountMembers.AsNoTracking()
            .FirstOrDefaultAsync(a => a.MemberID == id);
}
