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
    public int MemberId { get; set; }

    [BindProperty]
    public string MemberPassword { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("MemberId") != null)
            return RedirectToPage("/Products/Index");
        return Page();
    }

    public IActionResult OnPost()
    {
        var account = _accountService.GetAccountById(MemberId);
        if (account == null || account.MemberPassword != MemberPassword || (account.MemberRole != 1 && account.MemberRole != 2))
        {
            ErrorMessage = "You do not have permission to do this function!";
            return Page();
        }

        HttpContext.Session.SetInt32("MemberId", account.MemberId);
        HttpContext.Session.SetInt32("MemberRole", account.MemberRole);
        return RedirectToPage("/Products/Index");
    }
}
