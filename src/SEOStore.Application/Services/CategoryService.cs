using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SEOStore.Application.Features.Categories.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
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

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = Category.Create(
            dto.Name,
            dto.Description,
            dto.ImageUrl,
            dto.ParentCategoryId,
            dto.DisplayOrder,
            dto.MetaTitle,
            dto.MetaDescription);

        await _categoryRepository.AddAsync(category, cancellationToken);

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with id {dto.Id} was not found.");

        if (!string.Equals(category.Name, dto.Name, StringComparison.Ordinal))
        {
            category.Rename(dto.Name);
        }

        category.UpdateDetails(
            dto.Description,
            dto.ImageUrl,
            dto.DisplayOrder,
            dto.Published,
            dto.MetaTitle,
            dto.MetaDescription);

        category.SetParentId(dto.ParentCategoryId);

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return MapToDto(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with id {id} was not found.");

        await _categoryRepository.DeleteAsync(category, cancellationToken);
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
        DisplayOrder = category.DisplayOrder
    };
}
