using SEOStore.Domain.Common;
using System.Text.RegularExpressions;

namespace SEOStore.Domain.Entities.Catalog;

public class Category : SeoEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }

    public int? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }

    public int DisplayOrder { get; private set; }
    public bool Published { get; private set; } = true;

    public ICollection<Category> Children { get; private set; } = new List<Category>();
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category()
    {
    }

    public static Category Create(
        string name,
        string? description,
        string? imageUrl,
        int? parentCategoryId,
        int displayOrder,
        string? metaTitle,
        string? metaDescription)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Category name is required.");

        var category = new Category
        {
            Name = name.Trim(),
            Description = description,
            ImageUrl = imageUrl,
            ParentCategoryId = parentCategoryId,
            DisplayOrder = displayOrder,
            MetaTitle = metaTitle,
            MetaDescription = metaDescription,
            Published = true,
            CreatedAt = DateTime.UtcNow
        };

        category.Slug = GenerateSlug(name);
        return category;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Category name is required.");

        Name = name.Trim();
        Slug = GenerateSlug(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string? description,
        string? imageUrl,
        int displayOrder,
        bool published,
        string? metaTitle,
        string? metaDescription)
    {
        Description = description;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        Published = published;
        MetaTitle = metaTitle;
        MetaDescription = metaDescription;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParent(Category? parentCategory)
    {
        if (parentCategory is not null && parentCategory.Id == Id)
            throw new InvalidOperationException("A category cannot be its own parent.");

        ParentCategory = parentCategory;
        ParentCategoryId = parentCategory?.Id;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        Published = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Published = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "category";

        var slug = name.Trim().ToLowerInvariant();

        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "category" : slug;
    }

    public void SetParentId(int? parentCategoryId)
    {
        if (parentCategoryId == Id)
            throw new InvalidOperationException("A category cannot be its own parent.");

        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }
}