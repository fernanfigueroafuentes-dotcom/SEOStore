using SEOStore.Application.Common;
using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ISlugUniquenessService _slugUniqueness;
    private readonly ISlugRedirectService _slugRedirects;

    public ProductService(
        IProductRepository productRepository,
        ISlugUniquenessService slugUniqueness,
        ISlugRedirectService slugRedirects)
    {
        _productRepository = productRepository;
        _slugUniqueness = slugUniqueness;
        _slugRedirects = slugRedirects;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetBySlugAsync(slug, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetPublishedAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetPublishedBySlugAsync(slug, cancellationToken);
        return product is null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetPublishedByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetPublishedByCategoryAsync(categoryId, cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetFeaturedPublishedAsync(int take = 8, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetFeaturedPublishedAsync(take, cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = Product.Create(
            dto.Name,
            dto.SKU,
            dto.ShortDescription,
            dto.Description,
            dto.Price,
            dto.ShowPrice,
            dto.Featured,
            dto.Published,
            dto.ThumbnailUrl,
            dto.WhatsAppMessage,
            dto.CategoryId,
            dto.BrandId,
            dto.Stock);

        product.SetSeo(
            dto.MetaTitle,
            dto.MetaDescription,
            dto.OgTitle,
            dto.OgDescription,
            dto.ThumbnailUrl,
            canonicalUrl: null,
            dto.Index,
            dto.Follow);

        product.SetSlug(await _slugUniqueness.EnsureUniqueAsync(
            product.Slug,
            "product",
            SlugKind.Product,
            excludeId: null,
            cancellationToken));
        await _productRepository.AddAsync(product, cancellationToken);

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {dto.Id} was not found.");

        var oldSlug = product.Slug;
        if (!string.Equals(product.Name, dto.Name, StringComparison.Ordinal))
            product.Rename(dto.Name);

        product.UpdateDetails(
            dto.SKU,
            dto.ShortDescription,
            dto.Description,
            dto.Price,
            dto.ShowPrice,
            dto.Featured,
            dto.Published,
            dto.ThumbnailUrl,
            dto.WhatsAppMessage,
            dto.CategoryId,
            dto.BrandId);

        if (dto.Stock.HasValue)
            product.SetStock(dto.Stock);

        if (!string.Equals(oldSlug, product.Slug, StringComparison.Ordinal))
        {
            product.SetSlug(await _slugUniqueness.EnsureUniqueAsync(
                product.Slug,
                "product",
                SlugKind.Product,
                product.Id,
                cancellationToken));
        }

        await _productRepository.UpdateAsync(product, cancellationToken);

        if (!string.Equals(oldSlug, product.Slug, StringComparison.OrdinalIgnoreCase))
            await _slugRedirects.RecordChangeAsync(SlugKind.Product, oldSlug, product.Slug, cancellationToken);

        return MapToDto(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {id} was not found.");

        await _productRepository.DeleteAsync(product, cancellationToken);
    }

    private static ProductDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Slug = product.Slug,
        SKU = product.SKU,
        ShortDescription = product.ShortDescription,
        Description = product.Description,
        Price = product.Price,
        ShowPrice = product.ShowPrice,
        Featured = product.Featured,
        Published = product.Published,
        ThumbnailUrl = product.ThumbnailUrl,
        WhatsAppMessage = product.WhatsAppMessage,
        CategoryId = product.CategoryId,
        BrandId = product.BrandId,
        BrandName = product.Brand?.Name,
        Stock = product.Stock,
        CategoryName = product.Category?.Name,
        CategorySlug = product.Category?.Slug,
        MetaTitle = product.MetaTitle,
        MetaDescription = product.MetaDescription,
        CanonicalUrl = product.CanonicalUrl,
        OgTitle = product.OgTitle,
        OgDescription = product.OgDescription,
        OgImage = product.OgImage,
        Index = product.Index,
        Follow = product.Follow
    };
}
