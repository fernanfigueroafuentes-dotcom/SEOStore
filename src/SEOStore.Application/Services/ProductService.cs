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
            dto.BrandId);

        await _productRepository.AddAsync(product, cancellationToken);

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {dto.Id} was not found.");

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
}