using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IBrandRepository
{
    Task<List<Brand>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Brand?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Brand?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task AddAsync(Brand brand, CancellationToken cancellationToken = default);

    Task UpdateAsync(Brand brand, CancellationToken cancellationToken = default);

    Task DeleteAsync(Brand brand, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
}
