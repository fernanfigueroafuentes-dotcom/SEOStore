namespace SEOStore.Application.Features.Products.DTOs;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;

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

    public int? BrandId { get; set; }

    public int? Stock { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? OgTitle { get; set; }

    public string? OgDescription { get; set; }

    public bool Index { get; set; } = true;

    public bool Follow { get; set; } = true;
}
