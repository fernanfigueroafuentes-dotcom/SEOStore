using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Content.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Identity;
using SEOStore.Web.Html;
using SEOStore.Web.Models.Admin;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("admin/paginas")]
public class PagesAdminController : Controller
{
    private readonly IPageService _pageService;

    public PagesAdminController(IPageService pageService)
    {
        _pageService = pageService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Páginas", "/admin/paginas");
        return View(await _pageService.GetAllAsync(cancellationToken));
    }

    [HttpGet("nueva")]
    public IActionResult Create()
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva página", "/admin/paginas/nueva");
        return View("Edit", new PageForm());
    }

    [HttpPost("nueva")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PageForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva página", "/admin/paginas/nueva");
        if (!ModelState.IsValid)
            return View("Edit", form);

        await _pageService.CreateAsync(ToDto(form), cancellationToken);
        TempData["StatusMessage"] = "Página guardada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var page = await _pageService.GetByIdAsync(id, cancellationToken);
        if (page is null)
            return NotFound();

        ViewData["Seo"] = SeoPage.Admin("Editar página", $"/admin/paginas/{id}");
        return View(ToForm(page));
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PageForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Editar página", $"/admin/paginas/{id}");
        form.Id = id;
        if (!ModelState.IsValid)
            return View(form);

        try
        {
            await _pageService.UpdateAsync(ToDto(form), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Página actualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _pageService.DeleteAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Página eliminada.";
        return RedirectToAction(nameof(Index));
    }

    private static PageForm ToForm(PageDto page) => new()
    {
        Id = page.Id,
        Title = page.Title,
        Slug = page.Slug,
        Content = page.Content,
        Published = page.Published,
        MetaTitle = page.MetaTitle,
        MetaDescription = page.MetaDescription,
        Index = page.Index,
        Follow = page.Follow
    };

    private static UpsertPageDto ToDto(PageForm form) => new()
    {
        Id = form.Id,
        Title = form.Title,
        Slug = form.Slug,
        Content = ProductDescriptionHtml.Sanitize(form.Content),
        Published = form.Published,
        MetaTitle = form.MetaTitle,
        MetaDescription = form.MetaDescription,
        Index = form.Index,
        Follow = form.Follow
    };
}
