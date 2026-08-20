using SEOStore.Application.Common;
using SEOStore.Application.Features.Content.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Content;

namespace SEOStore.Application.Services;

public class PageService : IPageService
{
    private readonly IPageRepository _pageRepository;
    private readonly ISlugUniquenessService _slugUniqueness;
    private readonly ISlugRedirectService _slugRedirects;

    public PageService(
        IPageRepository pageRepository,
        ISlugUniquenessService slugUniqueness,
        ISlugRedirectService slugRedirects)
    {
        _pageRepository = pageRepository;
        _slugUniqueness = slugUniqueness;
        _slugRedirects = slugRedirects;
    }

    public async Task<IEnumerable<PageDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var pages = await _pageRepository.GetAllAsync(cancellationToken);
        return pages.Select(Map);
    }

    public async Task<IEnumerable<PageDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var pages = await _pageRepository.GetPublishedAsync(cancellationToken);
        return pages.Select(Map);
    }

    public async Task<PageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var page = await _pageRepository.GetByIdAsync(id, cancellationToken);
        return page is null ? null : Map(page);
    }

    public async Task<PageDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var page = await _pageRepository.GetPublishedBySlugAsync(slug, cancellationToken);
        return page is null ? null : Map(page);
    }

    public async Task<PageDto> CreateAsync(UpsertPageDto dto, CancellationToken cancellationToken = default)
    {
        var page = new Page
        {
            Title = RequireTitle(dto.Title),
            Content = dto.Content ?? string.Empty,
            Published = dto.Published,
            CreatedAt = DateTime.UtcNow,
            Index = dto.Index,
            Follow = dto.Follow
        };
        ApplySeo(page, dto);
        page.Slug = await _slugUniqueness.EnsureUniqueAsync(
            string.IsNullOrWhiteSpace(dto.Slug) ? page.Title : dto.Slug,
            "pagina",
            SlugKind.Page,
            excludeId: null,
            cancellationToken);
        await _pageRepository.AddAsync(page, cancellationToken);
        return Map(page);
    }

    public async Task<PageDto> UpdateAsync(UpsertPageDto dto, CancellationToken cancellationToken = default)
    {
        var page = await _pageRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Page {dto.Id} was not found.");

        var oldSlug = page.Slug;
        page.Title = RequireTitle(dto.Title);
        page.Content = dto.Content ?? string.Empty;
        page.Published = dto.Published;
        page.UpdatedAt = DateTime.UtcNow;
        ApplySeo(page, dto);
        page.Slug = await _slugUniqueness.EnsureUniqueAsync(
            string.IsNullOrWhiteSpace(dto.Slug) ? page.Title : dto.Slug,
            "pagina",
            SlugKind.Page,
            page.Id,
            cancellationToken);
        await _pageRepository.UpdateAsync(page, cancellationToken);

        if (!string.Equals(oldSlug, page.Slug, StringComparison.OrdinalIgnoreCase))
            await _slugRedirects.RecordChangeAsync(SlugKind.Page, oldSlug, page.Slug, cancellationToken);

        return Map(page);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var page = await _pageRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Page {id} was not found.");
        await _pageRepository.DeleteAsync(page, cancellationToken);
    }

    private static void ApplySeo(Page page, UpsertPageDto dto)
    {
        page.MetaTitle = string.IsNullOrWhiteSpace(dto.MetaTitle) ? page.Title : dto.MetaTitle.Trim();
        page.MetaDescription = string.IsNullOrWhiteSpace(dto.MetaDescription) ? null : dto.MetaDescription.Trim();
        page.OgTitle = page.MetaTitle;
        page.OgDescription = page.MetaDescription;
        page.Index = dto.Index;
        page.Follow = dto.Follow;
    }

    private static string RequireTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidOperationException("Page title is required.");
        return title.Trim();
    }

    private static PageDto Map(Page page) => new()
    {
        Id = page.Id,
        Title = page.Title,
        Slug = page.Slug,
        Content = page.Content,
        Published = page.Published,
        MetaTitle = page.MetaTitle,
        MetaDescription = page.MetaDescription,
        CanonicalUrl = page.CanonicalUrl,
        OgTitle = page.OgTitle,
        OgDescription = page.OgDescription,
        OgImage = page.OgImage,
        Index = page.Index,
        Follow = page.Follow
    };
}
