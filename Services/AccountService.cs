using BusinessObjects.Models;
using Repositories;
namespace Services;
public class AccountService : IAccountService {
    private readonly IAccountRepository _repo;
    public AccountService(IAccountRepository repo) => _repo = repo;
    public Task<AccountMember?> GetAccountByEmailAsync(string email) => _repo.GetAccountByEmailAsync(email);
    public Task<AccountMember?> GetAccountByIdAsync(string id) => _repo.GetAccountByIdAsync(id);
}