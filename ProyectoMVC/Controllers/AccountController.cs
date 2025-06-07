using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoMVC.Models;
using ProyectoMVC.Models.Data;
using ProyectoMVC.Models.ViewModels;
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return PartialView("~/Views/Account/_LoginModal.cshtml", new LoginVM());
    }

    [HttpGet]
    public IActionResult Register()
    {
        return PartialView("~/Views/Account/_RegisterModal.cshtml", new RegisterVM());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (!ModelState.IsValid)
        {
            // Si ModelState no es válido → devuelvo el mismo partial con las etiquetas de validación
            return PartialView("~/Views/Account/_RegisterModal.cshtml", model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Split('@')[0],
            Email = model.Email,
            name = model.Name,
            secondName = model.SecondName,
            registerDate = DateTime.UtcNow,
            lastLogin = DateTime.UtcNow,
            active = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            // Devolvemos JSON para que el AJAX sepa que fue exitoso
            return Json(new { success = true });
        }

        // Si CreateAsync falló (contraseña débil, email duplicado, etc.),
        // cargamos los errores en ModelState y devolvemos de nuevo el partial
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return PartialView("~/Views/Account/_RegisterModal.cshtml", model);
    }

    //[HttpGet]
    //public IActionResult Login() => View();
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model)
    {
        // 1) Si el modelo (email, password, etc.) está inválido, devolvemos el partial con errores
        if (!ModelState.IsValid)
        {
            return PartialView("~/Views/Account/_LoginModal.cshtml", model);
        }

        // 2) Intentamos firmar al usuario
        var result = await _signInManager.PasswordSignInAsync(
            model.Email.Split('@')[0],
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false // true para bloquear en caso de fallos de contraseña. Post produccion
        );

        if (result.Succeeded)
        {
            // 3) Login exitoso → devolvemos JSON para que el JS sepa redirigir
            return Json(new { success = true });
        }/*
        else if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "Usuario bloqueado por demasiados intentos fallidos. Intenta más tarde.");
        }
        else if (result.RequiresTwoFactor)
        {
            ModelState.AddModelError("", "Este usuario requiere autenticación en dos pasos.");
        }*/
        else
        {
            ModelState.AddModelError("", "Login inválido. Verificá email y contraseña.");
        }

        // 4) Si falla, devolvemos de nuevo el partial con los errores cargados en ModelState
        return PartialView("~/Views/Account/_LoginModal.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /*
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
            return RedirectToAction("Index", "Home");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ? View("ConfirmEmail") : View("Error");
    }*/
    /*
    [HttpGet]
    public async Task<IActionResult> TestCrearUsuario()
    {
    var user = new ApplicationUser
    {
        UserName = "prueba1",
        Email = "prueba1@mail.com",
        name = "Lu",
        secondName = "Test",
        registerDate = DateTime.UtcNow,
        lastLogin = DateTime.UtcNow,
        active = true
    };

    var result = await _userManager.CreateAsync(user, "Test123!");

    if (result.Succeeded)
    {
        return Content("✅ Usuario creado");
    }
    else
    {
        return Content("❌ " + string.Join(" / ", result.Errors.Select(e => e.Description)));
    }
    }*/

    // En tu AccountController (o donde tengas inyectado SignInManager<ApplicationUser>):
    [HttpGet]
    public async Task<IActionResult> TestCrearUsuario()
    {
        var user = new ApplicationUser
        {
            UserName = "prueba1@mail.com",
            Email = "prueba1@mail.com",
            name = "Lucas",
            secondName = "Test",
            registerDate = DateTime.UtcNow,
            lastLogin = DateTime.UtcNow,
            active = true
        };

        var result = await _userManager.CreateAsync(user, "Test123!");

        if (result.Succeeded)
        {
            return Content("✅ Usuario creado");
        }
        else
        {
            return Content("❌ " + string.Join(" / ", result.Errors.Select(e => e.Description)));
        }
    }

    // En tu AccountController (o donde tengas inyectado SignInManager<ApplicationUser>):
    [HttpGet]
    public async Task<IActionResult> TestLogin()
    {
        // 1) Credenciales “de prueba”:
        string emailDePrueba = "lucas";
        string passwordDePrueba = "Lucas2025!"; // debe coincidir con la contraseña real del usuario

        // 2) Intentamos el sign-in directamente con el SignInManager
        var resultado = await _signInManager.PasswordSignInAsync(
            emailDePrueba,
            passwordDePrueba,
            isPersistent: false,       // RememberMe = false
            lockoutOnFailure: false    // no bloqueamos por este test
        );

        // 3) Chequeamos si el login fue exitoso
        if (resultado.Succeeded)
        {
            // Si todo salió bien, devolvemos un “200 OK” con mensaje simple
            return Ok(new
            {
                success = true,
                mensaje = "TestLogin: login exitoso para " + emailDePrueba
            });
        }
        else if (resultado.IsLockedOut)
        {
            // Caso de usuario bloqueado
            return BadRequest(new
            {
                success = false,
                mensaje = "TestLogin: el usuario está bloqueado."
            });
        }
        else if (resultado.RequiresTwoFactor)
        {
            // Caso de 2FA activado
            return BadRequest(new
            {
                success = false,
                mensaje = "TestLogin: requiere autenticación de dos factores."
            });
        }
        else
        {
            // Cualquier otro fallo (credenciales inválidas, usuario no existe, etc.)
            return BadRequest(new
            {
                success = false,
                mensaje = "TestLogin: usuario o contraseña inválida."
            });
        }
    }
    [HttpGet]
    public IActionResult TestUsuarioLogueado()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // El usuario ya está autenticado
            // Devuelve, por ejemplo, el nombre de usuario
            return Ok(new
            {
                loggedIn = true,
                userName = User.Identity.Name  // aquí aparece el UserName (= claim “name” o “sub”, según cómo lo hayas configurado)
            });
        }
        else
        {
            // No hay nadie logueado
            return Ok(new
            {
                loggedIn = false
            });
        }
    }

}
