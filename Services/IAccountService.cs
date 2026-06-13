using BusinessObjects.Models;

namespace Services;

public interface IAccountService
{
    AccountMember? GetAccountById(int memberId);
}
