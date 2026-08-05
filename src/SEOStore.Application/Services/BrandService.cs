using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
        var slug = await GenerateUniqueSlugAsync(dto.Name, cancellationToken);

        var brand = new Brand
        {
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            LogoUrl = dto.LogoUrl,
            CreatedAt = DateTime.UtcNow
        };

        await _brandRepository.AddAsync(brand, cancellationToken);
        return MapToDto(brand);
    }

    public async Task<BrandDto> UpdateAsync(UpdateBrandDto dto, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Brand with id {dto.Id} was not found.");

        if (!string.Equals(brand.Name, dto.Name, StringComparison.Ordinal))
        {
            brand.Slug = await GenerateUniqueSlugAsync(dto.Name, cancellationToken, brand.Id);
        }

        brand.Name = dto.Name;
        brand.Description = dto.Description;
        brand.LogoUrl = dto.LogoUrl;
        brand.UpdatedAt = DateTime.UtcNow;

        await _brandRepository.UpdateAsync(brand, cancellationToken);
        return MapToDto(brand);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var brand = await _brandRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Brand with id {id} was not found.");

        await _brandRepository.DeleteAsync(brand, cancellationToken);
    }

    private static BrandDto MapToDto(Brand brand) => new()
    {
        Id = brand.Id,
        Name = brand.Name,
        Slug = brand.Slug,
        Description = brand.Description,
        LogoUrl = brand.LogoUrl
    };

    private async Task<string> GenerateUniqueSlugAsync(
        string name,
        CancellationToken cancellationToken,
        int? excludeId = null)
    {
        var baseSlug = GenerateSlug(name);
        var slug = baseSlug;
        var suffix = 1;

        while (await SlugExistsAsync(slug, excludeId, cancellationToken))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        return slug;
    }

    private async Task<bool> SlugExistsAsync(string slug, int? excludeId, CancellationToken cancellationToken)
    {
        var existing = await _brandRepository.GetBySlugAsync(slug, cancellationToken);
        return existing is not null && existing.Id != excludeId;
    }

    private static string GenerateSlug(string name)
    {
        var normalized = name.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        var slug = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", string.Empty);
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "brand" : slug;
    }
}
