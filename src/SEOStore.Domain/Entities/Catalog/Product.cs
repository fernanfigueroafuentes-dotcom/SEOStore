using SEOStore.Domain.Common;
using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Domain.Entities.Catalog;

public class Product : SeoEntity
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool ShowPrice { get; set; } = false;

    public bool Featured { get; set; }

    public bool Published { get; set; } = true;

    public string? ThumbnailUrl { get; set; }

    public string? WhatsAppMessage { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int? BrandId { get; set; }

    public Brand? Brand { get; set; }

    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];

    public ICollection<OrderItem> OrderItems { get; set; } = [];
}