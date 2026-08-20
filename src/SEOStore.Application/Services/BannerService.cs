using SEOStore.Application.Features.Banners.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Content;

namespace SEOStore.Application.Services;

public class BannerService : IBannerService
{
    private readonly IBannerRepository _bannerRepository;

    public BannerService(IBannerRepository bannerRepository)
    {
        _bannerRepository = bannerRepository;
    }

    public async Task<IEnumerable<BannerDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var banners = await _bannerRepository.GetAllAsync(cancellationToken);
        return banners.Select(Map);
    }

    public async Task<IEnumerable<BannerDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var banners = await _bannerRepository.GetActiveAsync(cancellationToken);
        return banners.Select(Map);
    }

    public async Task<BannerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var banner = await _bannerRepository.GetByIdAsync(id, cancellationToken);
        return banner is null ? null : Map(banner);
    }

    public async Task<BannerDto> CreateAsync(UpsertBannerDto dto, CancellationToken cancellationToken = default)
    {
        var banner = new Banner
        {
            CreatedAt = DateTime.UtcNow
        };
        Apply(banner, dto, requireImage: true);
        await _bannerRepository.AddAsync(banner, cancellationToken);
        return Map(banner);
    }

    public async Task<BannerDto> UpdateAsync(UpsertBannerDto dto, CancellationToken cancellationToken = default)
    {
        var banner = await _bannerRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Banner {dto.Id} was not found.");

        Apply(banner, dto, requireImage: false);
        banner.UpdatedAt = DateTime.UtcNow;
        await _bannerRepository.UpdateAsync(banner, cancellationToken);
        return Map(banner);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var banner = await _bannerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Banner {id} was not found.");

        await _bannerRepository.DeleteAsync(banner, cancellationToken);
    }

    private static void Apply(Banner banner, UpsertBannerDto dto, bool requireImage)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new InvalidOperationException("Banner title is required.");

        var imageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? banner.ImageUrl : dto.ImageUrl.Trim();
        if (requireImage && string.IsNullOrWhiteSpace(imageUrl))
            throw new InvalidOperationException("Banner image is required.");

        banner.Title = dto.Title.Trim();
        banner.Subtitle = string.IsNullOrWhiteSpace(dto.Subtitle) ? null : dto.Subtitle.Trim();
        banner.ImageUrl = imageUrl;
        banner.Link = string.IsNullOrWhiteSpace(dto.Link) ? null : dto.Link.Trim();
        banner.DisplayOrder = dto.DisplayOrder;
        banner.Active = dto.Active;
    }

    private static BannerDto Map(Banner banner) => new()
    {
        Id = banner.Id,
        Title = banner.Title,
        Subtitle = banner.Subtitle,
        ImageUrl = banner.ImageUrl,
        Link = banner.Link,
        DisplayOrder = banner.DisplayOrder,
        Active = banner.Active
    };
}
