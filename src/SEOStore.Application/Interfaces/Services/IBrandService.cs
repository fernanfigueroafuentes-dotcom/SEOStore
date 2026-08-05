using SEOStore.Application.Features.Brands.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BrandDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BrandDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<BrandDto> CreateAsync(CreateBrandDto dto, CancellationToken cancellationToken = default);

    Task<BrandDto> UpdateAsync(UpdateBrandDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
