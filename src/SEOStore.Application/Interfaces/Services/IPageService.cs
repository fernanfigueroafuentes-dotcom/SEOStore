using SEOStore.Application.Features.Content.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IPageService
{
    Task<IEnumerable<PageDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<PageDto>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<PageDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<PageDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<PageDto> CreateAsync(UpsertPageDto dto, CancellationToken cancellationToken = default);

    Task<PageDto> UpdateAsync(UpsertPageDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
