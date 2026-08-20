using SEOStore.Application.Common;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Application.Services;

public class SlugUniquenessService : ISlugUniquenessService
{
    private readonly ISlugRegistry _slugRegistry;

    public SlugUniquenessService(ISlugRegistry slugRegistry)
    {
        _slugRegistry = slugRegistry;
    }

    public async Task<string> EnsureUniqueAsync(
        string? requested,
        string fallback,
        SlugKind kind,
        int? excludeId,
        CancellationToken cancellationToken = default)
    {
        var baseSlug = ContentSlug.From(requested, fallback);
        var slug = baseSlug;
        var suffix = 2;

        while (await _slugRegistry.IsTakenAsync(slug, kind, excludeId, cancellationToken))
            slug = $"{baseSlug}-{suffix++}";

        return slug;
    }
}
