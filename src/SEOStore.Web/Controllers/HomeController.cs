using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Models;
using SEOStore.Web.Models.Store;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ISettingService _settingService;
    private readonly IBannerService _bannerService;

    public HomeController(
        IProductService productService,
        ICategoryService categoryService,
        ISettingService settingService,
        IBannerService bannerService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _settingService = settingService;
        _bannerService = bannerService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var site = await _settingService.GetCurrentAsync(cancellationToken);
        var categories = (await _categoryService.GetPublishedAsync(cancellationToken)).ToList();
        var featured = (await _productService.GetFeaturedPublishedAsync(8, cancellationToken)).ToList();
        var banners = (await _bannerService.GetActiveAsync(cancellationToken)).ToList();

        ViewData["Seo"] = new SeoPage
        {
            Title = site.SiteName,
            Description = $"Catálogo de {site.SiteName}.",
            CanonicalPath = "/",
            JsonLd = [JsonLd.Organization(site, SeoUrl.Absolute(Request, "/"))]
        };

        return View(new HomePageViewModel
        {
            Banners = banners,
            Categories = categories,
            Featured = featured
        });
    }

    public IActionResult Privacy()
    {
        ViewData["Seo"] = SeoPage.Admin("Privacidad", "/Home/Privacy");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        ViewData["Seo"] = SeoPage.Admin("Error", "/Home/Error");
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
