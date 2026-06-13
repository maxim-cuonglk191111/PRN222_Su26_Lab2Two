using BusinessObjects.Models;
using Repositories;

namespace Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public AccountMember? GetAccountById(int memberId) => _accountRepository.GetAccountById(memberId);
}
