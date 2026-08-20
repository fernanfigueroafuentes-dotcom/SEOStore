using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Settings.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Identity;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("admin/sitio")]
public class SettingsController : Controller
{
    private readonly ISettingService _settingService;

    public SettingsController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Edit(CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Sitio", "/admin/sitio");
        return View(await _settingService.GetEditableAsync(cancellationToken));
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SiteSettingsDto form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Sitio", "/admin/sitio");
        if (string.IsNullOrWhiteSpace(form.SiteName))
            ModelState.AddModelError(nameof(form.SiteName), "El nombre del sitio es obligatorio.");

        if (!ModelState.IsValid)
            return View(form);

        await _settingService.UpdateAsync(form, cancellationToken);
        TempData["StatusMessage"] = "Datos del sitio guardados.";
        return RedirectToAction(nameof(Edit));
    }
}
