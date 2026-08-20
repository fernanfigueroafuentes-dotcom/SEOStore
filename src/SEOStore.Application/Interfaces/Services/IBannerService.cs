using SEOStore.Application.Features.Banners.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IBannerService
{
    Task<IEnumerable<BannerDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<BannerDto>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<BannerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BannerDto> CreateAsync(UpsertBannerDto dto, CancellationToken cancellationToken = default);

    Task<BannerDto> UpdateAsync(UpsertBannerDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
