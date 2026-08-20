using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Models.Store;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

public class StoreController : Controller
{
    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly ICategoryService _categoryService;
    private readonly ISettingService _settingService;
    private readonly ISlugRedirectService _slugRedirects;

    public StoreController(
        IProductService productService,
        IProductImageService productImageService,
        ICategoryService categoryService,
        ISettingService settingService,
        ISlugRedirectService slugRedirects)
    {
        _productService = productService;
        _productImageService = productImageService;
        _categoryService = categoryService;
        _settingService = settingService;
        _slugRedirects = slugRedirects;
    }

    [HttpGet("/productos")]
    public async Task<IActionResult> Products(CancellationToken cancellationToken)
    {
        var products = (await _productService.GetPublishedAsync(cancellationToken)).ToList();
        var site = await _settingService.GetCurrentAsync(cancellationToken);

        ViewData["Seo"] = new SeoPage
        {
            Title = "Productos",
            Description = $"Catálogo de {site.SiteName}.",
            CanonicalPath = "/productos",
            JsonLd = [JsonLd.Organization(site, SeoUrl.Absolute(Request, "/"))]
        };

        return View(products);
    }

    [HttpGet("/producto/{slug}")]
    public async Task<IActionResult> Product(string slug, CancellationToken cancellationToken)
    {
        var product = await _productService.GetPublishedBySlugAsync(slug, cancellationToken);
        if (product is null)
            return await SlugRedirects.PermanentIfFound(this, _slugRedirects, $"/producto/{slug}", cancellationToken)
                ?? (IActionResult)NotFound();

        var images = (await _productImageService.GetAllByProductAsync(product.Id, cancellationToken)).ToList();
        var site = await _settingService.GetCurrentAsync(cancellationToken);
        var canonicalPath = $"/producto/{product.Slug}";
        var canonical = string.IsNullOrWhiteSpace(product.CanonicalUrl)
            ? SeoUrl.Absolute(Request, canonicalPath)
            : SeoUrl.Absolute(Request, product.CanonicalUrl);
        var ogImage = product.OgImage ?? product.ThumbnailUrl;
        var imageUrls = images.Select(image => SeoUrl.Absolute(Request, image.Url)).ToList();
        if (imageUrls.Count == 0 && !string.IsNullOrWhiteSpace(product.ThumbnailUrl))
            imageUrls.Add(SeoUrl.Absolute(Request, product.ThumbnailUrl));

        var crumbs = new List<(string Name, string Url)>
        {
            (site.SiteName, SeoUrl.Absolute(Request, "/")),
            ("Productos", SeoUrl.Absolute(Request, "/productos"))
        };

        if (!string.IsNullOrWhiteSpace(product.CategoryName) && !string.IsNullOrWhiteSpace(product.CategorySlug))
            crumbs.Add((product.CategoryName, SeoUrl.Absolute(Request, $"/categoria/{product.CategorySlug}")));

        crumbs.Add((product.Name, canonical));

        ViewData["Images"] = images;
        ViewData["Site"] = site;
        ViewData["Seo"] = new SeoPage
        {
            Title = product.MetaTitle ?? product.Name,
            Description = product.MetaDescription ?? product.ShortDescription,
            CanonicalPath = canonicalPath,
            OgTitle = product.OgTitle ?? product.Name,
            OgDescription = product.OgDescription ?? product.MetaDescription ?? product.ShortDescription,
            OgImage = ogImage,
            Index = product.Index,
            Follow = product.Follow,
            JsonLd =
            [
                JsonLd.Organization(site, SeoUrl.Absolute(Request, "/")),
                JsonLd.Product(product, imageUrls, canonical, site.Currency),
                JsonLd.Breadcrumb(crumbs)
            ]
        };

        return View(product);
    }

    [HttpGet("/categoria/{slug}")]
    public async Task<IActionResult> Category(string slug, CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetPublishedBySlugAsync(slug, cancellationToken);
        if (category is null)
            return await SlugRedirects.PermanentIfFound(this, _slugRedirects, $"/categoria/{slug}", cancellationToken)
                ?? (IActionResult)NotFound();

        var products = (await _productService.GetPublishedByCategoryAsync(category.Id, cancellationToken)).ToList();
        var site = await _settingService.GetCurrentAsync(cancellationToken);
        var canonicalPath = $"/categoria/{category.Slug}";
        var canonical = string.IsNullOrWhiteSpace(category.CanonicalUrl)
            ? SeoUrl.Absolute(Request, canonicalPath)
            : SeoUrl.Absolute(Request, category.CanonicalUrl);

        ViewData["Seo"] = new SeoPage
        {
            Title = category.MetaTitle ?? category.Name,
            Description = category.MetaDescription ?? category.Description,
            CanonicalPath = canonicalPath,
            OgTitle = category.OgTitle ?? category.Name,
            OgDescription = category.OgDescription ?? category.MetaDescription ?? category.Description,
            OgImage = category.OgImage ?? category.ImageUrl,
            Index = category.Index,
            Follow = category.Follow,
            JsonLd =
            [
                JsonLd.Organization(site, SeoUrl.Absolute(Request, "/")),
                JsonLd.Breadcrumb(
                [
                    (site.SiteName, SeoUrl.Absolute(Request, "/")),
                    ("Productos", SeoUrl.Absolute(Request, "/productos")),
                    (category.Name, canonical)
                ])
            ]
        };

        return View(new CategoryPageViewModel
        {
            Category = category,
            Products = products
        });
    }
}
