using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Catalog;

public class Brand : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Brand()
    {
    }

    public static Brand Create(string name, string? description, string? logoUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Brand name is required.");

        var brand = new Brand
        {
            Name = name.Trim(),
            Description = description,
            LogoUrl = logoUrl,
            CreatedAt = DateTime.UtcNow
        };

        brand.Slug = GenerateSlug(name);
        return brand;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Brand name is required.");

        Name = name.Trim();
        Slug = GenerateSlug(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string? description, string? logoUrl)
    {
        Description = description;
        LogoUrl = logoUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "brand";

        var slug = name.Trim().ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "brand" : slug;
    }
}