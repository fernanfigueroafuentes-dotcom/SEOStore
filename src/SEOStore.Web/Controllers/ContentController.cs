using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

public class ContentController : Controller
{
    private readonly IPageService _pageService;
    private readonly IBlogPostService _blogPostService;
    private readonly ISettingService _settingService;
    private readonly ISlugRedirectService _slugRedirects;

    public ContentController(
        IPageService pageService,
        IBlogPostService blogPostService,
        ISettingService settingService,
        ISlugRedirectService slugRedirects)
    {
        _pageService = pageService;
        _blogPostService = blogPostService;
        _settingService = settingService;
        _slugRedirects = slugRedirects;
    }

    [HttpGet("/pagina/{slug}")]
    public async Task<IActionResult> Page(string slug, CancellationToken cancellationToken)
    {
        var page = await _pageService.GetPublishedBySlugAsync(slug, cancellationToken);
        if (page is null)
            return await SlugRedirects.PermanentIfFound(this, _slugRedirects, $"/pagina/{slug}", cancellationToken)
                ?? (IActionResult)NotFound();

        var site = await _settingService.GetCurrentAsync(cancellationToken);
        var canonicalPath = $"/pagina/{page.Slug}";
        var canonical = SeoUrl.Absolute(Request, page.CanonicalUrl ?? canonicalPath);

        ViewData["Seo"] = new SeoPage
        {
            Title = page.MetaTitle ?? page.Title,
            Description = page.MetaDescription,
            CanonicalPath = canonicalPath,
            OgTitle = page.OgTitle ?? page.Title,
            OgDescription = page.OgDescription ?? page.MetaDescription,
            OgImage = page.OgImage,
            Index = page.Index,
            Follow = page.Follow,
            JsonLd =
            [
                JsonLd.Organization(site, SeoUrl.Absolute(Request, "/")),
                JsonLd.WebPage(page.Title, canonical, page.MetaDescription),
                JsonLd.Breadcrumb(
                [
                    (site.SiteName, SeoUrl.Absolute(Request, "/")),
                    (page.Title, canonical)
                ])
            ]
        };

        return View(page);
    }

    [HttpGet("/blog")]
    public async Task<IActionResult> Blog(CancellationToken cancellationToken)
    {
        var posts = (await _blogPostService.GetPublishedAsync(cancellationToken)).ToList();
        var site = await _settingService.GetCurrentAsync(cancellationToken);

        ViewData["Seo"] = new SeoPage
        {
            Title = "Blog",
            Description = $"Novedades de {site.SiteName}.",
            CanonicalPath = "/blog",
            JsonLd =
            [
                JsonLd.Organization(site, SeoUrl.Absolute(Request, "/")),
                JsonLd.Breadcrumb(
                [
                    (site.SiteName, SeoUrl.Absolute(Request, "/")),
                    ("Blog", SeoUrl.Absolute(Request, "/blog"))
                ])
            ]
        };

        return View(posts);
    }

    [HttpGet("/blog/{slug}")]
    public async Task<IActionResult> Post(string slug, CancellationToken cancellationToken)
    {
        var post = await _blogPostService.GetPublishedBySlugAsync(slug, cancellationToken);
        if (post is null)
            return await SlugRedirects.PermanentIfFound(this, _slugRedirects, $"/blog/{slug}", cancellationToken)
                ?? (IActionResult)NotFound();

        var site = await _settingService.GetCurrentAsync(cancellationToken);
        var canonicalPath = $"/blog/{post.Slug}";
        var canonical = SeoUrl.Absolute(Request, post.CanonicalUrl ?? canonicalPath);

        ViewData["Seo"] = new SeoPage
        {
            Title = post.MetaTitle ?? post.Title,
            Description = post.MetaDescription ?? post.Summary,
            CanonicalPath = canonicalPath,
            OgTitle = post.OgTitle ?? post.Title,
            OgDescription = post.OgDescription ?? post.MetaDescription ?? post.Summary,
            OgImage = post.OgImage ?? post.FeaturedImageUrl,
            Index = post.Index,
            Follow = post.Follow,
            JsonLd =
            [
                JsonLd.Organization(site, SeoUrl.Absolute(Request, "/")),
                JsonLd.BlogPosting(post.Title, canonical, post.Summary, post.Author, post.PublishedAt, post.FeaturedImageUrl),
                JsonLd.Breadcrumb(
                [
                    (site.SiteName, SeoUrl.Absolute(Request, "/")),
                    ("Blog", SeoUrl.Absolute(Request, "/blog")),
                    (post.Title, canonical)
                ])
            ]
        };

        return View(post);
    }
}
