using SEOStore.Application.Features.Content.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IBlogPostService
{
    Task<IEnumerable<BlogPostDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<BlogPostDto>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<BlogPostDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BlogPostDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<BlogPostDto> CreateAsync(UpsertBlogPostDto dto, CancellationToken cancellationToken = default);

    Task<BlogPostDto> UpdateAsync(UpsertBlogPostDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
