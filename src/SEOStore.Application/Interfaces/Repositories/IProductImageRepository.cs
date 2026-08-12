using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IProductImageRepository
{
    Task<List<ProductImage>> GetAllAsync(int productId, CancellationToken cancellationToken = default);

    Task<ProductImage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ProductImage?> GetPrimaryAsync(int productId, CancellationToken cancellationToken = default);

    Task AddAsync(ProductImage productImage, CancellationToken cancellationToken = default);

    Task UpdateAsync(ProductImage productImage, CancellationToken cancellationToken = default);

    Task DeleteAsync(ProductImage productImage, CancellationToken cancellationToken = default);
}
