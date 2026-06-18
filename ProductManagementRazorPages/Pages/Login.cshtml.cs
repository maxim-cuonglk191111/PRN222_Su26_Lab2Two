using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ProductManagementRazorPages.Pages;

public class LoginModel : PageModel
{
    private readonly IAccountService _accountService;

    public LoginModel(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [BindProperty]
    public string EmailAddress { get; set; } = string.Empty;

    [BindProperty]
    public string MemberPassword { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserId") != null)
            return RedirectToPage("/Products/Index");
        return Page();
    }

    private string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var account = await _accountService.GetAccountByEmailAsync(EmailAddress);
        if (account == null || (account.MemberPassword != MemberPassword && account.MemberPassword != HashPassword(MemberPassword)))
        {
            ErrorMessage = "You do not have permission to do this function!";
            return Page();
        }

        HttpContext.Session.SetString("UserId", account.MemberID);
        HttpContext.Session.SetInt32("MemberRole", account.MemberRole);
        return RedirectToPage("/Products/Index");
    }
}
