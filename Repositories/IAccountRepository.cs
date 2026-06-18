using BusinessObjects.Models;
namespace Repositories;
public interface IAccountRepository {
    Task<AccountMember?> GetAccountByEmailAsync(string email);
    Task<AccountMember?> GetAccountByIdAsync(string id);
}