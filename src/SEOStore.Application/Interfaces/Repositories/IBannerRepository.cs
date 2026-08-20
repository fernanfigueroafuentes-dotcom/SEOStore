using SEOStore.Domain.Entities.Content;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IBannerRepository
{
    Task<List<Banner>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<Banner>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<Banner?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task AddAsync(Banner banner, CancellationToken cancellationToken = default);

    Task UpdateAsync(Banner banner, CancellationToken cancellationToken = default);

    Task DeleteAsync(Banner banner, CancellationToken cancellationToken = default);
}
