using SEOStore.Application.Features.Brands.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<IEnumerable<BrandDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var brands = await _brandRepository.GetAllAsync(cancellationToken);
        return brands.Select(MapToDto);
    }

    public async Task<BrandDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(id, cancellationToken);
        return brand is null ? null : MapToDto(brand);
    }

    public async Task<BrandDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetBySlugAsync(slug, cancellationToken);
        return brand is null ? null : MapToDto(brand);
    }

    public async Task<BrandDto> CreateAsync(CreateBrandDto dto, CancellationToken cancellationToken = default)
    {
        var brand = Brand.Create(dto.Name, dto.Description, dto.LogoUrl);
        brand.SetSlug(await EnsureUniqueSlugAsync(brand.Slug, excludeId: null, cancellationToken));
        await _brandRepository.AddAsync(brand, cancellationToken);
        return MapToDto(brand);
    }

    public async Task<BrandDto> UpdateAsync(UpdateBrandDto dto, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Brand with id {dto.Id} was not found.");

        var oldSlug = brand.Slug;
        if (!string.Equals(brand.Name, dto.Name, StringComparison.Ordinal))
            brand.Rename(dto.Name);

        brand.UpdateDetails(dto.Description, dto.LogoUrl);

        if (!string.Equals(oldSlug, brand.Slug, StringComparison.Ordinal))
            brand.SetSlug(await EnsureUniqueSlugAsync(brand.Slug, brand.Id, cancellationToken));

        await _brandRepository.UpdateAsync(brand, cancellationToken);

        return MapToDto(brand);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Brand with id {id} was not found.");

        await _brandRepository.DeleteAsync(brand, cancellationToken);
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug, int? excludeId, CancellationToken cancellationToken)
    {
        var baseSlug = string.IsNullOrWhiteSpace(slug) ? "brand" : slug;
        var unique = baseSlug;
        var suffix = 2;
        while (await _brandRepository.ExistsBySlugAsync(unique, excludeId, cancellationToken))
            unique = $"{baseSlug}-{suffix++}";
        return unique;
    }

    private static BrandDto MapToDto(Brand brand) => new()
    {
        Id = brand.Id,
        Name = brand.Name,
        Slug = brand.Slug,
        Description = brand.Description,
        LogoUrl = brand.LogoUrl
    };
}