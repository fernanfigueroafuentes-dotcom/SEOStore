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
[Route("admin/blog")]
public class BlogAdminController : Controller
{
    private readonly IBlogPostService _blogPostService;

    public BlogAdminController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Blog", "/admin/blog");
        return View(await _blogPostService.GetAllAsync(cancellationToken));
    }

    [HttpGet("nueva")]
    public IActionResult Create()
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva nota", "/admin/blog/nueva");
        return View("Edit", new BlogPostForm());
    }

    [HttpPost("nueva")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPostForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Nueva nota", "/admin/blog/nueva");
        if (!ModelState.IsValid)
            return View("Edit", form);

        await _blogPostService.CreateAsync(ToDto(form), cancellationToken);
        TempData["StatusMessage"] = "Nota guardada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var post = await _blogPostService.GetByIdAsync(id, cancellationToken);
        if (post is null)
            return NotFound();

        ViewData["Seo"] = SeoPage.Admin("Editar nota", $"/admin/blog/{id}");
        return View(ToForm(post));
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BlogPostForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Editar nota", $"/admin/blog/{id}");
        form.Id = id;
        if (!ModelState.IsValid)
            return View(form);

        try
        {
            await _blogPostService.UpdateAsync(ToDto(form), cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Nota actualizada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/eliminar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _blogPostService.DeleteAsync(id, cancellationToken);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["StatusMessage"] = "Nota eliminada.";
        return RedirectToAction(nameof(Index));
    }

    private static BlogPostForm ToForm(BlogPostDto post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Slug = post.Slug,
        Summary = post.Summary,
        Content = post.Content,
        FeaturedImageUrl = post.FeaturedImageUrl,
        Author = post.Author,
        DisplayOrder = post.DisplayOrder,
        Published = post.Published,
        MetaTitle = post.MetaTitle,
        MetaDescription = post.MetaDescription,
        Index = post.Index,
        Follow = post.Follow
    };

    private static UpsertBlogPostDto ToDto(BlogPostForm form) => new()
    {
        Id = form.Id,
        Title = form.Title,
        Slug = form.Slug,
        Summary = form.Summary ?? string.Empty,
        Content = ProductDescriptionHtml.Sanitize(form.Content),
        FeaturedImageUrl = form.FeaturedImageUrl,
        Author = form.Author,
        DisplayOrder = form.DisplayOrder,
        Published = form.Published,
        MetaTitle = form.MetaTitle,
        MetaDescription = form.MetaDescription,
        Index = form.Index,
        Follow = form.Follow
    };
}
