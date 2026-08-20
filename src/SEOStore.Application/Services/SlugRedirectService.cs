using SEOStore.Application.Common;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Seo;

namespace SEOStore.Application.Services;

public class SlugRedirectService : ISlugRedirectService
{
    private readonly ISlugRedirectRepository _redirectRepository;
    private readonly ISlugRegistry _slugRegistry;

    public SlugRedirectService(ISlugRedirectRepository redirectRepository, ISlugRegistry slugRegistry)
    {
        _redirectRepository = redirectRepository;
        _slugRegistry = slugRegistry;
    }

    public async Task RecordChangeAsync(SlugKind kind, string oldSlug, string newSlug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(oldSlug) || string.IsNullOrWhiteSpace(newSlug))
            return;

        if (string.Equals(oldSlug, newSlug, StringComparison.OrdinalIgnoreCase))
            return;

        var oldPath = PublicSlug.Path(kind, oldSlug);
        var newPath = PublicSlug.Path(kind, newSlug);

        var occupying = await _redirectRepository.GetByOldPathAsync(newPath, cancellationToken);
        if (occupying is not null)
            await _redirectRepository.DeleteAsync(occupying, cancellationToken);

        foreach (var inbound in await _redirectRepository.GetByNewPathAsync(oldPath, cancellationToken))
        {
            if (string.Equals(inbound.OldPath, newPath, StringComparison.OrdinalIgnoreCase))
            {
                await _redirectRepository.DeleteAsync(inbound, cancellationToken);
                continue;
            }

            inbound.NewPath = newPath;
            inbound.UpdatedAt = DateTime.UtcNow;
            await _redirectRepository.UpdateAsync(inbound, cancellationToken);
        }

        var existing = await _redirectRepository.GetByOldPathAsync(oldPath, cancellationToken);
        if (existing is null)
        {
            await _redirectRepository.AddAsync(new SlugRedirect
            {
                OldPath = oldPath,
                NewPath = newPath,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
            return;
        }

        existing.NewPath = newPath;
        existing.UpdatedAt = DateTime.UtcNow;
        await _redirectRepository.UpdateAsync(existing, cancellationToken);
    }

    public async Task<string?> ResolveLiveAsync(string requestedPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(requestedPath))
            return null;

        var current = requestedPath.Trim();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var hop = 0; hop < 8; hop++)
        {
            var redirect = await _redirectRepository.GetByOldPathAsync(current, cancellationToken);
            if (redirect is null)
                break;

            if (!seen.Add(current))
                return null;

            current = redirect.NewPath;
        }

        if (string.Equals(current, requestedPath, StringComparison.OrdinalIgnoreCase))
            return null;

        return await _slugRegistry.IsLivePathAsync(current, cancellationToken) ? current : null;
    }
}
