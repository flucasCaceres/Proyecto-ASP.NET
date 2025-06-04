using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoMVC.Models;
using ProyectoMVC.Models.ViewModels;
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpGet]
    public IActionResult LoginModal()
    {
        return PartialView("~/Views/Account/_LoginModal.cshtml", new LoginVM());
    }

    [HttpGet]
    public IActionResult RegisterModal()
    {
        return PartialView("~/Views/Account/_RegisterModal.cshtml", new RegisterVM());
    }



    //[HttpGet]
    //public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> RegisterModal(RegisterVM model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email.Split('@')[0],
                Email = model.Email,
                EmailConfirmed = false,
                name = model.Name,
                secondName = model.SecondName,
                registerDate = DateTime.UtcNow,
                lastLogin = DateTime.UtcNow,
                active = true,
                NormalizedUserName = model.Email.Split('@')[0].ToUpper(),
                NormalizedEmail = model.Email.ToUpper(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        return PartialView("~/Views/Account/_RegisterModal.cshtml", model);
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Login inválido.");
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
