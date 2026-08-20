using SEOStore.Application.Common;

namespace SEOStore.Application.Interfaces.Services;

public interface ISlugUniquenessService
{
    Task<string> EnsureUniqueAsync(
        string? requested,
        string fallback,
        SlugKind kind,
        int? excludeId,
        CancellationToken cancellationToken = default);
}
