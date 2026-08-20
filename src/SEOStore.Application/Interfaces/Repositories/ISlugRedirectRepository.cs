using SEOStore.Domain.Entities.Seo;

namespace SEOStore.Application.Interfaces.Repositories;

public interface ISlugRedirectRepository
{
    Task<SlugRedirect?> GetByOldPathAsync(string oldPath, CancellationToken cancellationToken = default);

    Task<List<SlugRedirect>> GetByNewPathAsync(string newPath, CancellationToken cancellationToken = default);

    Task AddAsync(SlugRedirect redirect, CancellationToken cancellationToken = default);

    Task UpdateAsync(SlugRedirect redirect, CancellationToken cancellationToken = default);

    Task DeleteAsync(SlugRedirect redirect, CancellationToken cancellationToken = default);
}
