using SEOStore.Application.Common;
using SEOStore.Application.Features.Categories.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISlugUniquenessService _slugUniqueness;
    private readonly ISlugRedirectService _slugRedirects;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ISlugUniquenessService slugUniqueness,
        ISlugRedirectService slugRedirects)
    {
        _categoryRepository = categoryRepository;
        _slugUniqueness = slugUniqueness;
        _slugRedirects = slugRedirects;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetBySlugAsync(slug, cancellationToken);
        return category is null ? null : MapToDto(category);
    }

    public async Task<IEnumerable<CategoryDto>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetPublishedAsync(cancellationToken);
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetPublishedBySlugAsync(slug, cancellationToken);
        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var parentCategoryId = await GetValidatedParentCategoryIdAsync(
            dto.ParentCategoryId,
            cancellationToken);

        var category = Category.Create(
            dto.Name,
            dto.Description,
            dto.ImageUrl,
            parentCategoryId,
            dto.DisplayOrder,
            dto.MetaTitle,
            dto.MetaDescription);

        category.UpdateDetails(
            dto.Description,
            dto.ImageUrl,
            dto.DisplayOrder,
            dto.Published,
            dto.MetaTitle,7
            dto.MetaDescription,
            dto.Index,
            dto.Follow);

        category.SetSlug(await _slugUniqueness.EnsureUniqueAsync(
            category.Slug,
            "category",
            SlugKind.Category,
            excludeId: null,
            cancellationToken));

        await _categoryRepository.AddAsync(category, cancellationToken);

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with id {dto.Id} was not found.");

        var oldSlug = category.Slug;
        if (!string.Equals(category.Name, dto.Name, StringComparison.Ordinal))
            category.Rename(dto.Name);

        category.UpdateDetails(
            dto.Description,
            dto.ImageUrl,
            dto.DisplayOrder,
            dto.Published,
            dto.MetaTitle,
            dto.MetaDescription,
            dto.Index,
            dto.Follow);

        var parentCategoryId = await GetValidatedParentCategoryIdAsync(
            dto.ParentCategoryId,
            cancellationToken);

        category.SetParentId(parentCategoryId);

        if (!string.Equals(oldSlug, category.Slug, StringComparison.Ordinal))
        {
            category.SetSlug(await _slugUniqueness.EnsureUniqueAsync(
                category.Slug,
                "category",
                SlugKind.Category,
                category.Id,
                cancellationToken));
        }

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        if (!string.Equals(oldSlug, category.Slug, StringComparison.OrdinalIgnoreCase))
            await _slugRedirects.RecordChangeAsync(SlugKind.Category, oldSlug, category.Slug, cancellationToken);

        return MapToDto(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with id {id} was not found.");

        await _categoryRepository.DeleteAsync(category, cancellationToken);
    }

    private async Task<int?> GetValidatedParentCategoryIdAsync(
        int? parentCategoryId,
        CancellationToken cancellationToken)
    {
        if (parentCategoryId is null or 0)
            return null;

        if (parentCategoryId < 0)
            throw new ArgumentException("Parent category id must be a positive number.", nameof(parentCategoryId));

        var parentCategory = await _categoryRepository.GetByIdAsync(parentCategoryId.Value, cancellationToken);
        if (parentCategory is null)
            throw new KeyNotFoundException("The requested parent category was not found.");

        return parentCategory.Id;
    }

    private static CategoryDto MapToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Slug = category.Slug,
        Description = category.Description,
        ImageUrl = category.ImageUrl,
        ParentCategoryId = category.ParentCategoryId,
        Published = category.Published,
        DisplayOrder = category.DisplayOrder,
        MetaTitle = category.MetaTitle,
        MetaDescription = category.MetaDescription,
        CanonicalUrl = category.CanonicalUrl,
        OgTitle = category.OgTitle,
        OgDescription = category.OgDescription,
        OgImage = category.OgImage,
        Index = category.Index,
        Follow = category.Follow
    };
}
