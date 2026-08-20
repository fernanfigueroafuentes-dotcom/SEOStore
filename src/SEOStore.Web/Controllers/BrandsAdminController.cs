using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Brands.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Identity;
using SEOStore.Web.Models.Admin;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("admin/marcas")]
public class BrandsAdminController : Controller
{
    private readonly IBrandService _brandService;

    public BrandsAdminController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Marcas", "/admin/marcas");
        return View(await _brandService.GetAllAsync(cancellationToken));
    }

    [HttpGet("nueva")]
    public IActionResult Create()
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva marca", "/admin/marcas/nueva");
        return View("Edit", new BrandForm());
    }

    [HttpPost("nueva")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BrandForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva marca", "/admin/marcas/nueva");
        if (!ModelState.IsValid)
            return View("Edit", form);

        await _brandService.CreateAsync(ToCreateDto(form), cancellationToken);
        TempData["StatusMessage"] = "Marca guardada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var brand = await _brandService.GetByIdAsync(id, cancellationToken);
        if (brand is null)
            return NotFound();

        ViewData["Seo"] = SeoPage.Admin("Editar marca", $"/admin/marcas/{id}");
        return View(ToForm(brand));
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BrandForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Editar marca", $"/admin/marcas/{id}");
        form.Id = id;
        if (!ModelState.IsValid)
            return View(form);

        try
        {
            await _brandService.UpdateAsync(ToUpdateDto(form), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Marca actualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _brandService.DeleteAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Marca eliminada.";
        return RedirectToAction(nameof(Index));
    }

    private static BrandForm ToForm(BrandDto brand) => new()
    {
        Id = brand.Id,
        Name = brand.Name,
        Description = brand.Description,
        LogoUrl = brand.LogoUrl
    };

    private static CreateBrandDto ToCreateDto(BrandForm form) => new()
    {
        Name = form.Name,
        Description = form.Description,
        LogoUrl = form.LogoUrl
    };

    private static UpdateBrandDto ToUpdateDto(BrandForm form) => new()
    {
        Id = form.Id,
        Name = form.Name,
        Description = form.Description,
        LogoUrl = form.LogoUrl
    };
}
