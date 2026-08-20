using SEOStore.Application.Features.Products.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductDto>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<ProductDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductDto>> GetPublishedByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProductDto>> GetFeaturedPublishedAsync(int take = 8, CancellationToken cancellationToken = default);

    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default);

    Task<ProductDto> UpdateAsync(UpdateProductDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
