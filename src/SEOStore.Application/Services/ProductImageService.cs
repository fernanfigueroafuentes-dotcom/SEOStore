using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Services;

public class ProductImageService : IProductImageService
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IImageStorageService _imageStorageService;

    public ProductImageService(
        IProductImageRepository productImageRepository,
        IProductRepository productRepository,
        IImageStorageService imageStorageService)
    {
        _productImageRepository = productImageRepository;
        _productRepository = productRepository;
        _imageStorageService = imageStorageService;
    }

    public async Task<IEnumerable<ProductImageDto>> GetAllByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {productId} was not found.");

        var images = await _productImageRepository.GetAllAsync(product.Id, cancellationToken);
        return images.Select(MapToDto);
    }

    public async Task<ProductImageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetByIdAsync(id, cancellationToken);
        return image is null ? null : MapToDto(image);
    }

    public async Task<ProductImageDto> CreateAsync(CreateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {dto.ProductId} was not found.");

        if (string.IsNullOrWhiteSpace(dto.Url))
            throw new InvalidOperationException("Image URL is required.");

        var existingPrimary = await _productImageRepository.GetPrimaryAsync(product.Id, cancellationToken);
        if (dto.IsPrimary && existingPrimary is not null)
        {
            existingPrimary.IsPrimary = false;
            existingPrimary.UpdatedAt = DateTime.UtcNow;
            await _productImageRepository.UpdateAsync(existingPrimary, cancellationToken);
        }

        var image = new ProductImage
        {
            ProductId = product.Id,
            Url = dto.Url.Trim(),
            PublicId = dto.PublicId?.Trim() ?? string.Empty,
            Alt = dto.Alt,
            IsPrimary = dto.IsPrimary,
            DisplayOrder = dto.DisplayOrder,
            CreatedAt = DateTime.UtcNow
        };

        if (dto.IsPrimary)
            image.IsPrimary = true;

        await _productImageRepository.AddAsync(image, cancellationToken);

        return MapToDto(image);
    }

    public async Task<ProductImageDto> UpdateAsync(UpdateProductImageDto dto, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product image with id {dto.Id} was not found.");

        var product = await _productRepository.GetByIdAsync(dto.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with id {dto.ProductId} was not found.");

        if (string.IsNullOrWhiteSpace(dto.Url))
            throw new InvalidOperationException("Image URL is required.");

        if (dto.IsPrimary)
        {
            var currentPrimary = await _productImageRepository.GetPrimaryAsync(product.Id, cancellationToken);
            if (currentPrimary is not null && currentPrimary.Id != image.Id)
            {
                currentPrimary.IsPrimary = false;
                currentPrimary.UpdatedAt = DateTime.UtcNow;
                await _productImageRepository.UpdateAsync(currentPrimary, cancellationToken);
            }
        }

        image.ProductId = product.Id;
        image.Url = dto.Url.Trim();
        image.PublicId = dto.PublicId?.Trim() ?? string.Empty;
        image.Alt = dto.Alt;
        image.IsPrimary = dto.IsPrimary;
        image.DisplayOrder = dto.DisplayOrder;
        image.UpdatedAt = DateTime.UtcNow;

        await _productImageRepository.UpdateAsync(image, cancellationToken);

        return MapToDto(image);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product image with id {id} was not found.");

        if (!string.IsNullOrWhiteSpace(image.PublicId))
        {
            await _imageStorageService.DeleteAsync(image.PublicId, cancellationToken);
        }

        await _productImageRepository.DeleteAsync(image, cancellationToken);
    }

    public async Task<ProductImageDto> SetPrimaryAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _productImageRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product image with id {id} was not found.");

        var currentPrimary = await _productImageRepository.GetPrimaryAsync(image.ProductId, cancellationToken);
        if (currentPrimary is not null && currentPrimary.Id != image.Id)
        {
            currentPrimary.IsPrimary = false;
            currentPrimary.UpdatedAt = DateTime.UtcNow;
            await _productImageRepository.UpdateAsync(currentPrimary, cancellationToken);
        }

        image.IsPrimary = true;
        image.UpdatedAt = DateTime.UtcNow;
        await _productImageRepository.UpdateAsync(image, cancellationToken);

        return MapToDto(image);
    }

    private static ProductImageDto MapToDto(ProductImage image) => new()
    {
        Id = image.Id,
        ProductId = image.ProductId,
        Url = image.Url,
        PublicId = image.PublicId,
        Alt = image.Alt,
        IsPrimary = image.IsPrimary,
        DisplayOrder = image.DisplayOrder
    };
}
