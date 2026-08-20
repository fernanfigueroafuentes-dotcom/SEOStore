using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SEOStore.Application.Features.Brands.DTOs;
using SEOStore.Application.Features.Categories.DTOs;
using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Identity;
using SEOStore.Infrastructure.Services;
using SEOStore.Web.Html;
using SEOStore.Web.Models.Catalog;
using SEOStore.Web.Seo;

namespace SEOStore.Web.Controllers;

public class CatalogController : Controller
{
    private static readonly HashSet<string> AllowedPhotoExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private readonly IProductService _productService;
    private readonly IProductImageService _productImageService;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly IImageStorageService _imageStorageService;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(
        IProductService productService,
        IProductImageService productImageService,
        ICategoryService categoryService,
        IBrandService brandService,
        IImageStorageService imageStorageService,
        ILogger<CatalogController> logger)
    {
        _productService = productService;
        _productImageService = productImageService;
        _categoryService = categoryService;
        _brandService = brandService;
        _imageStorageService = imageStorageService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true || !User.IsInRole(AppRoles.Admin))
            return RedirectPermanent("/productos");

        ViewData["Seo"] = SeoPage.Admin("Productos", "/Catalog");
        var products = await _productService.GetAllAsync(cancellationToken);
        return View(products);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null || !product.Published)
            return NotFound();

        return RedirectPermanent($"/producto/{product.Slug}");
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Nuevo producto", "/Catalog/Create");
        var form = new CreateProductForm();
        await PopulateLookupsAsync(form, cancellationToken);
        return View(form);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(20_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 20_000_000)]
    public async Task<IActionResult> Create(CreateProductForm form, CancellationToken cancellationToken)
    {
        ViewData["Seo"] = SeoPage.Admin("Nuevo producto", "/Catalog/Create");
        await PopulateLookupsAsync(form, cancellationToken);

        var photos = GetUploadedPhotos();
        foreach (var photo in photos)
        {
            var extension = Path.GetExtension(photo.FileName);
            if (!AllowedPhotoExtensions.Contains(extension))
                ModelState.AddModelError(nameof(form.Photos), "Solo se permiten imágenes JPG, PNG, WEBP o GIF.");
        }

        if (!ModelState.IsValid)
            return View(form);

        try
        {
            var categoryId = await ResolveCategoryIdAsync(form, cancellationToken);
            var sku = string.IsNullOrWhiteSpace(form.SKU)
                ? $"P-{DateTime.UtcNow:yyyyMMddHHmmssfff}"
                : form.SKU.Trim();

            var product = await _productService.CreateAsync(new CreateProductDto
            {
                Name = form.Name,
                SKU = sku,
                ShortDescription = form.ShortDescription,
                Description = ProductDescriptionHtml.Sanitize(form.Description),
                Price = form.Price,
                ShowPrice = form.ShowPrice,
                Featured = form.Featured,
                Published = form.Published,
                WhatsAppMessage = form.WhatsAppMessage,
                CategoryId = categoryId,
                BrandId = await ResolveBrandIdAsync(form, cancellationToken),
                MetaTitle = form.MetaTitle,
                MetaDescription = form.MetaDescription,
                Index = form.Index,
                Follow = form.Follow
            }, cancellationToken);

            string? thumbnailUrl = null;
            var displayOrder = 0;
            var photoErrors = 0;

            string? photoError = null;

            foreach (var photo in photos)
            {
                try
                {
                    await using var buffer = new MemoryStream();
                    await photo.CopyToAsync(buffer, cancellationToken);
                    buffer.Position = 0;

                    var photoName = ImageAssetNames.FromProduct(product.Name, displayOrder + 1);
                    var uploaded = await _imageStorageService.UploadAsync(
                        buffer,
                        photo.FileName,
                        $"products/{product.Id}",
                        photoName,
                        cancellationToken);

                    var isPrimary = thumbnailUrl is null;
                    await _productImageService.CreateAsync(new CreateProductImageDto
                    {
                        ProductId = product.Id,
                        Url = uploaded.Url,
                        PublicId = uploaded.PublicId,
                        Alt = string.IsNullOrWhiteSpace(form.PhotoAlt) ? product.Name : form.PhotoAlt.Trim(),
                        IsPrimary = isPrimary,
                        DisplayOrder = displayOrder++
                    }, cancellationToken);

                    if (isPrimary)
                        thumbnailUrl = uploaded.Url;
                }
                catch (Exception exception)
                {
                    photoErrors++;
                    photoError = MapPhotoError(exception);
                    _logger.LogError(exception, "Could not save photo {FileName} for product {ProductId}.", photo.FileName, product.Id);
                }
            }

            if (!string.IsNullOrWhiteSpace(thumbnailUrl))
            {
                await _productService.UpdateAsync(new UpdateProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    SKU = product.SKU,
                    ShortDescription = product.ShortDescription,
                    Description = product.Description,
                    Price = product.Price,
                    ShowPrice = product.ShowPrice,
                    Featured = product.Featured,
                    Published = product.Published,
                    ThumbnailUrl = thumbnailUrl,
                    WhatsAppMessage = product.WhatsAppMessage,
                    CategoryId = product.CategoryId,
                    BrandId = product.BrandId
                }, cancellationToken);
            }

            TempData["StatusMessage"] = photoErrors > 0
                ? photoError ?? "Producto guardado, pero las fotos no se subieron a Cloudinary."
                : photos.Count == 0
                    ? "Producto guardado sin fotos."
                    : "Producto guardado.";

            return product.Published
                ? Redirect($"/producto/{product.Slug}")
                : RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not create product {ProductName}.", form.Name);
            ModelState.AddModelError(string.Empty, "No se pudo guardar el producto. Revisa los datos e inténtalo de nuevo.");
            return View(form);
        }
    }

    private List<IFormFile> GetUploadedPhotos()
    {
        var named = Request.Form.Files.GetFiles("Photos");
        if (named.Count > 0)
            return named.Where(file => file.Length > 0).ToList();

        return Request.Form.Files
            .Where(file => file.Length > 0)
            .ToList();
    }

    private async Task PopulateLookupsAsync(CreateProductForm form, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        form.Categories = categories
            .Select(category => new SelectListItem(category.Name, category.Id.ToString(), category.Id == form.CategoryId))
            .ToList();

        var brands = await _brandService.GetAllAsync(cancellationToken);
        form.Brands = brands
            .Select(brand => new SelectListItem(brand.Name, brand.Id.ToString(), brand.Id == form.BrandId))
            .ToList();
    }

    private async Task<int> ResolveCategoryIdAsync(CreateProductForm form, CancellationToken cancellationToken)
    {
        if (form.CategoryId is > 0)
            return form.CategoryId.Value;

        var name = string.IsNullOrWhiteSpace(form.NewCategoryName) ? "General" : form.NewCategoryName.Trim();
        var existing = (await _categoryService.GetAllAsync(cancellationToken))
            .FirstOrDefault(category => string.Equals(category.Name, name, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
            return existing.Id;

        var created = await _categoryService.CreateAsync(new CreateCategoryDto
        {
            Name = name,
            MetaTitle = form.NewCategoryMetaTitle,
            MetaDescription = form.NewCategoryMetaDescription
        }, cancellationToken);
        return created.Id;
    }

    private async Task<int?> ResolveBrandIdAsync(CreateProductForm form, CancellationToken cancellationToken)
    {
        if (form.BrandId is > 0)
            return form.BrandId.Value;

        if (string.IsNullOrWhiteSpace(form.NewBrandName))
            return null;

        var name = form.NewBrandName.Trim();
        var existing = (await _brandService.GetAllAsync(cancellationToken))
            .FirstOrDefault(brand => string.Equals(brand.Name, name, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
            return existing.Id;

        var created = await _brandService.CreateAsync(new CreateBrandDto { Name = name }, cancellationToken);
        return created.Id;
    }

    private static string MapPhotoError(Exception exception)
    {
        var message = exception.Message ?? string.Empty;
        if (message.Contains("missing permissions", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("Create/Upload", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
        {
            return "Cloudinary rechazó la foto: la API key no tiene permiso de Create/Upload. " +
                   "En console.cloudinary.com → Settings → API Keys, edita la key y asígnale Master Admin " +
                   "(o un rol que pueda subir assets). Reinicia la app y vuelve a subir las fotos.";
        }

        return "Producto guardado, pero las fotos no se subieron a Cloudinary.";
    }
}
