using BusinessObjects.Models;
using DataAccessObjects;

namespace Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountDAO _accountDAO;

    public AccountRepository(AccountDAO accountDAO)
    {
        _accountDAO = accountDAO;
    }

    public AccountMember? GetAccountById(int memberId) => _accountDAO.GetAccountById(memberId);
}
