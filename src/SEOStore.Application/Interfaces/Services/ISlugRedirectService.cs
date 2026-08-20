using SEOStore.Application.Common;

namespace SEOStore.Application.Interfaces.Services;

public interface ISlugRedirectService
{
    Task RecordChangeAsync(SlugKind kind, string oldSlug, string newSlug, CancellationToken cancellationToken = default);

    Task<string?> ResolveLiveAsync(string requestedPath, CancellationToken cancellationToken = default);
}
