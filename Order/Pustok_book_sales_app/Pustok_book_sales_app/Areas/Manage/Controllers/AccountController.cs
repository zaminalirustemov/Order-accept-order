using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok_book_sales_app.Areas.Manage.ViewModels;
using Pustok_book_sales_app.Models;

namespace Pustok_book_sales_app.Areas.Manage.Controllers;
[Area("Manage")]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signinManager;

    public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signinManager)
    {
        _userManager = userManager;
        _signinManager = signinManager;
    }
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(AdminLoginViewModel adminLoginVM)
    {
        if (!ModelState.IsValid) return View();
        AppUser admin = await _userManager.FindByNameAsync(adminLoginVM.UserName);
        if (admin is null)
        {
            ModelState.AddModelError("", "Username or password is false");
            return View();
        }

        var result= await _signinManager.PasswordSignInAsync(admin, adminLoginVM.Password, false, false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password is false");
            return View();
        }

        return RedirectToAction("index", "dashboard");
    }

    public async Task<IActionResult> Logout()
    {
        await _signinManager.SignOutAsync();

        return RedirectToAction("login", "account");
    }
}

