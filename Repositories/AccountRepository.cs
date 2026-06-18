using BusinessObjects.Models;
using DataAccessObjects;
namespace Repositories;
public class AccountRepository : IAccountRepository {
    private readonly AccountDAO _dao;
    public AccountRepository(AccountDAO dao) => _dao = dao;
    public Task<AccountMember?> GetAccountByEmailAsync(string email) => _dao.GetAccountByEmailAsync(email);
    public Task<AccountMember?> GetAccountByIdAsync(string id) => _dao.GetAccountByIdAsync(id);
}