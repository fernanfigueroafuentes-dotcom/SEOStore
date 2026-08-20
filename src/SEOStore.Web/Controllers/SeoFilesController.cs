using System.Text;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

public class SeoFilesController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IPageService _pageService;
    private readonly IBlogPostService _blogPostService;

    public SeoFilesController(
        IProductService productService,
        ICategoryService categoryService,
        IPageService pageService,
        IBlogPostService blogPostService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _pageService = pageService;
        _blogPostService = blogPostService;
    }

    [HttpGet("/robots.txt")]
    public IActionResult Robots()
    {
        var origin = $"{Request.Scheme}://{Request.Host}";
        var body =
            """
            User-agent: *
            Allow: /
            Disallow: /Catalog
            Disallow: /Catalog/
            Disallow: /cuenta
            Disallow: /cuenta/
            Disallow: /api/
            Disallow: /admin
            Disallow: /admin/
            Disallow: /swagger
            Disallow: /swagger/

            """ + $"Sitemap: {origin}/sitemap.xml\n";

        return Content(body, "text/plain", Encoding.UTF8);
    }

    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Sitemap(CancellationToken cancellationToken)
    {
        var urls = new List<(string Path, DateTime? Lastmod)>
        {
            ("/", DateTime.UtcNow),
            ("/productos", DateTime.UtcNow),
            ("/blog", DateTime.UtcNow)
        };

        var categories = await _categoryService.GetPublishedAsync(cancellationToken);
        foreach (var category in categories.Where(item => item.Index))
            urls.Add(($"/categoria/{category.Slug}", null));

        var products = await _productService.GetPublishedAsync(cancellationToken);
        foreach (var product in products.Where(item => item.Index))
            urls.Add(($"/producto/{product.Slug}", null));

        var pages = await _pageService.GetPublishedAsync(cancellationToken);
        foreach (var page in pages.Where(item => item.Index))
            urls.Add(($"/pagina/{page.Slug}", null));

        var posts = await _blogPostService.GetPublishedAsync(cancellationToken);
        foreach (var post in posts.Where(item => item.Index))
            urls.Add(($"/blog/{post.Slug}", null));

        var xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");

        foreach (var (path, lastmod) in urls)
        {
            xml.AppendLine("  <url>");
            xml.AppendLine($"    <loc>{System.Security.SecurityElement.Escape(SeoUrl.Absolute(Request, path))}</loc>");
            if (lastmod is not null)
                xml.AppendLine($"    <lastmod>{lastmod:yyyy-MM-dd}</lastmod>");
            xml.AppendLine("  </url>");
        }

        xml.AppendLine("</urlset>");
        return Content(xml.ToString(), "application/xml", Encoding.UTF8);
    }
}
