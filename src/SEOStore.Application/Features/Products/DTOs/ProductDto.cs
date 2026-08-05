namespace SEOStore.Application.Features.Products.DTOs;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public bool ShowPrice { get; set; }

    public bool Featured { get; set; }

    public bool Published { get; set; }

    public string? ThumbnailUrl { get; set; }

    public string? WhatsAppMessage { get; set; }

    public int CategoryId { get; set; }

    public int? BrandId { get; set; }
}
