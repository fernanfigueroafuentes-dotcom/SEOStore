using SEOStore.Domain.Common;
using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Domain.Entities.Catalog;

public class Product : SeoEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string SKU { get; private set; } = string.Empty;
    public string? ShortDescription { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public bool ShowPrice { get; private set; } = false;
    public bool Featured { get; private set; }
    public bool Published { get; private set; } = true;
    public string? ThumbnailUrl { get; private set; }
    public string? WhatsAppMessage { get; private set; }

    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public int? BrandId { get; private set; }
    public Brand? Brand { get; private set; }

    public ICollection<ProductImage> Images { get; private set; } = [];
    public ICollection<CartItem> CartItems { get; private set; } = [];
    public ICollection<OrderItem> OrderItems { get; private set; } = [];

    private Product()
    {
    }

    public static Product Create(
        string name,
        string sku,
        string? shortDescription,
        string? description,
        decimal price,
        bool showPrice,
        bool featured,
        bool published,
        string? thumbnailUrl,
        string? whatsappMessage,
        int categoryId,
        int? brandId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Product name is required.");

        if (categoryId <= 0)
            throw new InvalidOperationException("Category is required.");

        if (price < 0)
            throw new InvalidOperationException("Price cannot be negative.");

        var product = new Product
        {
            Name = name.Trim(),
            SKU = sku.Trim(),
            ShortDescription = shortDescription,
            Description = description,
            Price = price,
            ShowPrice = showPrice,
            Featured = featured,
            Published = published,
            ThumbnailUrl = thumbnailUrl,
            WhatsAppMessage = whatsappMessage,
            CategoryId = categoryId,
            BrandId = brandId,
            CreatedAt = DateTime.UtcNow
        };

        product.Slug = GenerateSlug(name);
        return product;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException("Product name is required.");

        Name = name.Trim();
        Slug = GenerateSlug(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string sku,
        string? shortDescription,
        string? description,
        decimal price,
        bool showPrice,
        bool featured,
        bool published,
        string? thumbnailUrl,
        string? whatsappMessage,
        int categoryId,
        int? brandId)
    {
        if (categoryId <= 0)
            throw new InvalidOperationException("Category is required.");

        if (price < 0)
            throw new InvalidOperationException("Price cannot be negative.");

        SKU = sku.Trim();
        ShortDescription = shortDescription;
        Description = description;
        Price = price;
        ShowPrice = showPrice;
        Featured = featured;
        Published = published;
        ThumbnailUrl = thumbnailUrl;
        WhatsAppMessage = whatsappMessage;
        CategoryId = categoryId;
        BrandId = brandId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBrand(int? brandId)
    {
        BrandId = brandId;
        UpdatedAt = DateTime.UtcNow;
    }

    public static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "product";

        var slug = name.Trim().ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "product" : slug;
    }
}