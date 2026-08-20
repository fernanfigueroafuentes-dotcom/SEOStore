using SEOStore.Application.Common;

namespace SEOStore.Application.Interfaces.Repositories;

public interface ISlugRegistry
{
    Task<bool> IsTakenAsync(string slug, SlugKind? excludeKind, int? excludeId, CancellationToken cancellationToken = default);

    Task<bool> IsLivePathAsync(string path, CancellationToken cancellationToken = default);
}
