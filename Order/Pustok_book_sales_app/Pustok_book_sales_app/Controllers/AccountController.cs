using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok_book_sales_app.Areas.Manage.ViewModels;
using Pustok_book_sales_app.Models;
using Pustok_book_sales_app.ViewModel;

namespace Pustok_book_sales_app.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly PustokDbContext _pustokDbContext;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager,PustokDbContext pustokDbContext,SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _pustokDbContext = pustokDbContext;
        _signInManager = signInManager;
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(MemberRegisterViewModel memberRegisterVM)
    {
        if (!ModelState.IsValid) return View();

        AppUser user = null;

        user = _pustokDbContext.Users.FirstOrDefault(x => x.NormalizedUserName == memberRegisterVM.Username.ToUpper());
        if (user is not null)
        {
            ModelState.AddModelError("Username", "Already exist");
            return View();
        }
        user = _pustokDbContext.Users.FirstOrDefault(x => x.NormalizedEmail == memberRegisterVM.Email.ToUpper());
        if (user is not null)
        {
            ModelState.AddModelError("Email", "Already exist");
            return View();
        }

        user = new AppUser
        {
            FullName = memberRegisterVM.Fullname,
            UserName = memberRegisterVM.Username,
            Email = memberRegisterVM.Email
        };

        var result = await _userManager.CreateAsync(user, memberRegisterVM.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        await _userManager.AddToRoleAsync(user, "Member");
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("index","home");
    }


    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(MemberLoginViewModel memberLoginVM)
    {
        if (!ModelState.IsValid) return View();
        AppUser user = await _userManager.FindByNameAsync(memberLoginVM.UserName);
        if (user is null)
        {
            ModelState.AddModelError("", "Username or password is false");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(user, memberLoginVM.Password, false, false);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password is false");
            return View();
        }

        return RedirectToAction("index", "home");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("login", "account");
    }

    public async Task<IActionResult> Profile()
    {
        AppUser member = null;
        if (User.Identity.IsAuthenticated)
        {
            member = await _userManager.FindByNameAsync(User.Identity.Name);
        }

        List<Order> orders = _pustokDbContext.Orders.Where(x=>x.AppUserId==member.Id).ToList();
        return View(orders);
    }
}
