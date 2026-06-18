using BusinessObjects.Models;
using Repositories;
namespace Services;
public interface IAccountService {
    Task<AccountMember?> GetAccountByEmailAsync(string email);
    Task<AccountMember?> GetAccountByIdAsync(string id);
}