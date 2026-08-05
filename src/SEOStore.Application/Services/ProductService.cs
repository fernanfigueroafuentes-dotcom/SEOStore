using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
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

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var slug = await GenerateUniqueSlugAsync(dto.Name, cancellationToken);

        var product = new Product
        {
            Name = dto.Name,
            Slug = slug,
            SKU = dto.SKU,
            ShortDescription = dto.ShortDescription,
            Description = dto.Description,
            Price = dto.Price,
            ShowPrice = dto.ShowPrice,
            Featured = dto.Featured,
            Published = dto.Published,
            ThumbnailUrl = dto.ThumbnailUrl,
            WhatsAppMessage = dto.WhatsAppMessage,
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product, cancellationToken);
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {dto.Id} was not found.");

        if (!string.Equals(product.Name, dto.Name, StringComparison.Ordinal))
        {
            product.Slug = await GenerateUniqueSlugAsync(dto.Name, cancellationToken, product.Id);
        }

        product.Name = dto.Name;
        product.SKU = dto.SKU;
        product.ShortDescription = dto.ShortDescription;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.ShowPrice = dto.ShowPrice;
        product.Featured = dto.Featured;
        product.Published = dto.Published;
        product.ThumbnailUrl = dto.ThumbnailUrl;
        product.WhatsAppMessage = dto.WhatsAppMessage;
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product, cancellationToken);
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
        BrandId = product.BrandId
    };

    private async Task<string> GenerateUniqueSlugAsync(
        string name,
        CancellationToken cancellationToken,
        int? excludeId = null)
    {
        var baseSlug = GenerateSlug(name);
        var slug = baseSlug;
        var suffix = 1;

        while (await SlugExistsAsync(slug, excludeId, cancellationToken))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }

    private async Task<bool> SlugExistsAsync(string slug, int? excludeId, CancellationToken cancellationToken)
    {
        var existing = await _productRepository.GetBySlugAsync(slug, cancellationToken);
        return existing is not null && existing.Id != excludeId;
    }

    private static string GenerateSlug(string name)
    {
        var normalized = name.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        var slug = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", string.Empty);
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "product" : slug;
    }
}
