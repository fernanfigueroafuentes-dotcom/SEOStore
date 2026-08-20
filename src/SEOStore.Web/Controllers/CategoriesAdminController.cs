using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Categories.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Identity;
using SEOStore.Web.Models.Admin;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("admin/categorias")]
public class CategoriesAdminController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesAdminController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Categorías", "/admin/categorias");
        return View(await _categoryService.GetAllAsync(cancellationToken));
    }

    [HttpGet("nueva")]
    public IActionResult Create()
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva categoría", "/admin/categorias/nueva");
        return View("Edit", new CategoryForm());
    }

    [HttpPost("nueva")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva categoría", "/admin/categorias/nueva");
        if (!ModelState.IsValid)
            return View("Edit", form);

        await _categoryService.CreateAsync(ToCreateDto(form), cancellationToken);
        TempData["StatusMessage"] = "Categoría guardada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category is null)
            return NotFound();

        ViewData["Seo"] = SeoPage.Admin("Editar categoría", $"/admin/categorias/{id}");
        return View(ToForm(category));
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Editar categoría", $"/admin/categorias/{id}");
        form.Id = id;
        if (!ModelState.IsValid)
            return View(form);

        try
        {
            await _categoryService.UpdateAsync(ToUpdateDto(form), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Categoría actualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.DeleteAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Categoría eliminada.";
        return RedirectToAction(nameof(Index));
    }

    private static CategoryForm ToForm(CategoryDto category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        MetaTitle = category.MetaTitle,
        MetaDescription = category.MetaDescription,
        Published = category.Published,
        Index = category.Index,
        Follow = category.Follow,
        DisplayOrder = category.DisplayOrder
    };

    private static CreateCategoryDto ToCreateDto(CategoryForm form) => new()
    {
        Name = form.Name,
        Description = form.Description,
        DisplayOrder = form.DisplayOrder,
        Published = form.Published,
        MetaTitle = form.MetaTitle,
        MetaDescription = form.MetaDescription,
        Index = form.Index,
        Follow = form.Follow
    };

    private static UpdateCategoryDto ToUpdateDto(CategoryForm form) => new()
    {
        Id = form.Id,
        Name = form.Name,
        Description = form.Description,
        DisplayOrder = form.DisplayOrder,
        Published = form.Published,
        MetaTitle = form.MetaTitle,
        MetaDescription = form.MetaDescription,
        Index = form.Index,
        Follow = form.Follow
    };
}
