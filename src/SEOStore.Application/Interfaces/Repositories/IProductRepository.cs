using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<List<Product>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<Product?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<List<Product>> GetPublishedByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<List<Product>> GetFeaturedPublishedAsync(int take, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    Task DeleteAsync(Product product, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
