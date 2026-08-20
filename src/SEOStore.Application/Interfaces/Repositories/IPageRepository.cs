using SEOStore.Domain.Entities.Content;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IPageRepository
{
    Task<List<Page>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<Page>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<Page?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Page?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task AddAsync(Page page, CancellationToken cancellationToken = default);

    Task UpdateAsync(Page page, CancellationToken cancellationToken = default);

    Task DeleteAsync(Page page, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);
}
