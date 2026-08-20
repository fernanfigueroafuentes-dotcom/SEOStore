using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Domain.Identity;
using SEOStore.Infrastructure.Identity;
using SEOStore.Web.Models.Account;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

[Route("cuenta")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("ingresar")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["Seo"] = SeoPage.Admin("Ingresar", "/cuenta/ingresar");
        return View(new LoginForm { ReturnUrl = returnUrl });
    }

    [HttpPost("ingresar")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginForm form)
    {
        ViewData["Seo"] = SeoPage.Admin("Ingresar", "/cuenta/ingresar");

        if (!ModelState.IsValid)
            return View(form);

        var user = await _userManager.FindByEmailAsync(form.Email.Trim());
        if (user is null || !await _userManager.IsInRoleAsync(user, AppRoles.Admin))
        {
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            return View(form);
        }

        var result = await _signInManager.PasswordSignInAsync(user, form.Password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            return View(form);
        }

        if (!string.IsNullOrWhiteSpace(form.ReturnUrl) && Url.IsLocalUrl(form.ReturnUrl))
            return Redirect(form.ReturnUrl);

        return RedirectToAction("Create", "Catalog");
    }

    [HttpPost("salir")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
