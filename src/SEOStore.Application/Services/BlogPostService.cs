using SEOStore.Application.Common;
using SEOStore.Application.Features.Content.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Content;

namespace SEOStore.Application.Services;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly ISlugUniquenessService _slugUniqueness;
    private readonly ISlugRedirectService _slugRedirects;

    public BlogPostService(
        IBlogPostRepository blogPostRepository,
        ISlugUniquenessService slugUniqueness,
        ISlugRedirectService slugRedirects)
    {
        _blogPostRepository = blogPostRepository;
        _slugUniqueness = slugUniqueness;
        _slugRedirects = slugRedirects;
    }

    public async Task<IEnumerable<BlogPostDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var posts = await _blogPostRepository.GetAllAsync(cancellationToken);
        return posts.Select(Map);
    }

    public async Task<IEnumerable<BlogPostDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var posts = await _blogPostRepository.GetPublishedAsync(cancellationToken);
        return posts.Select(Map);
    }

    public async Task<BlogPostDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _blogPostRepository.GetByIdAsync(id, cancellationToken);
        return post is null ? null : Map(post);
    }

    public async Task<BlogPostDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var post = await _blogPostRepository.GetPublishedBySlugAsync(slug, cancellationToken);
        return post is null ? null : Map(post);
    }

    public async Task<BlogPostDto> CreateAsync(UpsertBlogPostDto dto, CancellationToken cancellationToken = default)
    {
        var post = new BlogPost
        {
            Title = RequireTitle(dto.Title),
            Summary = dto.Summary?.Trim() ?? string.Empty,
            Content = dto.Content ?? string.Empty,
            FeaturedImageUrl = dto.FeaturedImageUrl?.Trim(),
            Author = dto.Author?.Trim(),
            DisplayOrder = dto.DisplayOrder,
            Published = dto.Published,
            PublishedAt = dto.Published ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow
        };
        ApplySeo(post, dto);
        post.Slug = await _slugUniqueness.EnsureUniqueAsync(
            string.IsNullOrWhiteSpace(dto.Slug) ? post.Title : dto.Slug,
            "nota",
            SlugKind.BlogPost,
            excludeId: null,
            cancellationToken);
        await _blogPostRepository.AddAsync(post, cancellationToken);
        return Map(post);
    }

    public async Task<BlogPostDto> UpdateAsync(UpsertBlogPostDto dto, CancellationToken cancellationToken = default)
    {
        var post = await _blogPostRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Blog post {dto.Id} was not found.");

        var oldSlug = post.Slug;
        post.Title = RequireTitle(dto.Title);
        post.Summary = dto.Summary?.Trim() ?? string.Empty;
        post.Content = dto.Content ?? string.Empty;
        post.FeaturedImageUrl = dto.FeaturedImageUrl?.Trim();
        post.Author = dto.Author?.Trim();
        post.DisplayOrder = dto.DisplayOrder;
        if (dto.Published && !post.Published)
            post.PublishedAt = DateTime.UtcNow;
        if (!dto.Published)
            post.PublishedAt = post.PublishedAt;
        post.Published = dto.Published;
        post.UpdatedAt = DateTime.UtcNow;
        ApplySeo(post, dto);
        post.Slug = await _slugUniqueness.EnsureUniqueAsync(
            string.IsNullOrWhiteSpace(dto.Slug) ? post.Title : dto.Slug,
            "nota",
            SlugKind.BlogPost,
            post.Id,
            cancellationToken);
        await _blogPostRepository.UpdateAsync(post, cancellationToken);

        if (!string.Equals(oldSlug, post.Slug, StringComparison.OrdinalIgnoreCase))
            await _slugRedirects.RecordChangeAsync(SlugKind.BlogPost, oldSlug, post.Slug, cancellationToken);

        return Map(post);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var post = await _blogPostRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Blog post {id} was not found.");
        await _blogPostRepository.DeleteAsync(post, cancellationToken);
    }

    private static void ApplySeo(BlogPost post, UpsertBlogPostDto dto)
    {
        post.MetaTitle = string.IsNullOrWhiteSpace(dto.MetaTitle) ? post.Title : dto.MetaTitle.Trim();
        post.MetaDescription = string.IsNullOrWhiteSpace(dto.MetaDescription)
            ? (string.IsNullOrWhiteSpace(post.Summary) ? null : post.Summary)
            : dto.MetaDescription.Trim();
        post.OgTitle = post.MetaTitle;
        post.OgDescription = post.MetaDescription;
        post.OgImage = post.FeaturedImageUrl;
        post.Index = dto.Index;
        post.Follow = dto.Follow;
    }

    private static string RequireTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new InvalidOperationException("Post title is required.");
        return title.Trim();
    }

    private static BlogPostDto Map(BlogPost post) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Slug = post.Slug,
        Summary = post.Summary,
        Content = post.Content,
        FeaturedImageUrl = post.FeaturedImageUrl,
        Published = post.Published,
        PublishedAt = post.PublishedAt,
        Author = post.Author,
        DisplayOrder = post.DisplayOrder,
        MetaTitle = post.MetaTitle,
        MetaDescription = post.MetaDescription,
        CanonicalUrl = post.CanonicalUrl,
        OgTitle = post.OgTitle,
        OgDescription = post.OgDescription,
        OgImage = post.OgImage,
        Index = post.Index,
        Follow = post.Follow
    };
}
