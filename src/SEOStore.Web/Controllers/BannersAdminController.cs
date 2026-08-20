using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Banners.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Identity;
using SEOStore.Infrastructure.Services;
using SEOStore.Web.Models.Admin;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("admin/banners")]
public class BannersAdminController : Controller
{
    private static readonly HashSet<string> AllowedPhotoExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private readonly IBannerService _bannerService;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<BannersAdminController> _logger;

    public BannersAdminController(
        IBannerService bannerService,
        IImageStorageService imageStorageService,
        ILogger<BannersAdminController> logger)
    {
        _bannerService = bannerService;
        _imageStorageService = imageStorageService;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Banners", "/admin/banners");
        return View(await _bannerService.GetAllAsync(cancellationToken));
    }

    [HttpGet("nuevo")]
    public IActionResult Create()
    {
        ViewData["Seo"] = SeoPage.Admin("Nuevo banner", "/admin/banners/nuevo");
        return View("Edit", new BannerForm());
    }

    [HttpPost("nuevo")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Create(BannerForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Nuevo banner", "/admin/banners/nuevo");
        var imageUrl = await ResolveImageUrlAsync(form, existingUrl: null, cancellationToken);
        if (string.IsNullOrWhiteSpace(imageUrl))
            ModelState.AddModelError(nameof(form.Photo), "Subí una foto o pegá la URL de la imagen.");

        if (!ModelState.IsValid)
            return View("Edit", form);

        try
        {
            await _bannerService.CreateAsync(ToDto(form, imageUrl), cancellationToken);
        }
        catch (InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, "No se pudo guardar el banner. Revisá título e imagen.");
            return View("Edit", form);
        }

        TempData["StatusMessage"] = "Banner guardado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var banner = await _bannerService.GetByIdAsync(id, cancellationToken);
        if (banner is null)
            return NotFound();

        ViewData["Seo"] = SeoPage.Admin("Editar banner", $"/admin/banners/{id}");
        return View(ToForm(banner));
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(20_000_000)]
    public async Task<IActionResult> Edit(int id, BannerForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Editar banner", $"/admin/banners/{id}");
        form.Id = id;
        var existing = await _bannerService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound();

        var imageUrl = await ResolveImageUrlAsync(form, existing.ImageUrl, cancellationToken);
        if (string.IsNullOrWhiteSpace(imageUrl))
            ModelState.AddModelError(nameof(form.Photo), "Subí una foto o pegá la URL de la imagen.");

        if (!ModelState.IsValid)
            return View(form);

        try
        {
            await _bannerService.UpdateAsync(ToDto(form, imageUrl), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Banner actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _bannerService.DeleteAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Banner eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string?> ResolveImageUrlAsync(BannerForm form, string? existingUrl, CancellationToken cancellationToken)
    {
        if (form.Photo is { Length: > 0 })
        {
            var extension = Path.GetExtension(form.Photo.FileName);
            if (!AllowedPhotoExtensions.Contains(extension))
            {
                ModelState.AddModelError(nameof(form.Photo), "Solo se permiten imágenes JPG, PNG, WEBP o GIF.");
                return existingUrl;
            }

            try
            {
                await using var buffer = new MemoryStream();
                await form.Photo.CopyToAsync(buffer, cancellationToken);
                buffer.Position = 0;
                var uploaded = await _imageStorageService.UploadAsync(
                    buffer,
                    form.Photo.FileName,
                    "banners",
                    ImageAssetNames.FromFileName(form.Photo.FileName, form.Title),
                    cancellationToken);
                return uploaded.Url;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not upload banner image.");
                ModelState.AddModelError(nameof(form.Photo), "No se pudo subir la imagen.");
                return existingUrl;
            }
        }

        return string.IsNullOrWhiteSpace(form.ImageUrl) ? existingUrl : form.ImageUrl.Trim();
    }

    private static BannerForm ToForm(BannerDto banner) => new()
    {
        Id = banner.Id,
        Title = banner.Title,
        Subtitle = banner.Subtitle,
        ImageUrl = banner.ImageUrl,
        Link = banner.Link,
        DisplayOrder = banner.DisplayOrder,
        Active = banner.Active
    };

    private static UpsertBannerDto ToDto(BannerForm form, string? imageUrl) => new()
    {
        Id = form.Id,
        Title = form.Title,
        Subtitle = form.Subtitle,
        ImageUrl = imageUrl,
        Link = form.Link,
        DisplayOrder = form.DisplayOrder,
        Active = form.Active
    };
}
