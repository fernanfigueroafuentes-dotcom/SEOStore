using SEOStore.Application.Features.Products.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IProductImageService
{
    Task<IEnumerable<ProductImageDto>> GetAllByProductAsync(int productId, CancellationToken cancellationToken = default);

    Task<ProductImageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductImageDto> CreateAsync(CreateProductImageDto dto, CancellationToken cancellationToken = default);

    Task<ProductImageDto> UpdateAsync(UpdateProductImageDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductImageDto> SetPrimaryAsync(int id, CancellationToken cancellationToken = default);
}
