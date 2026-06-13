using BusinessObjects;
using BusinessObjects.Models;

namespace DataAccessObjects;

public class AccountDAO
{
    private readonly MyStoreContext _context;

    public AccountDAO(MyStoreContext context)
    {
        _context = context;
    }

    public AccountMember? GetAccountById(int memberId) =>
        _context.AccountMembers.FirstOrDefault(a => a.MemberId == memberId);
}
