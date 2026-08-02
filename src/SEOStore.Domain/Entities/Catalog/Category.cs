using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Catalog;

public class Category : SeoEntity
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    // Jerarquía
    public int? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }

    public int DisplayOrder { get; set; }

    public bool Published { get; set; } = true;

    public ICollection<Category> Children { get; set; } = new List<Category>();

    public ICollection<Product> Products { get; set; } = new List<Product>();
}