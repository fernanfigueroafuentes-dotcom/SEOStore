namespace SEOStore.Application.Features.Products.DTOs;

public class ProductImageDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Url { get; set; } = string.Empty;

    public string PublicId { get; set; } = string.Empty;

    public string? Alt { get; set; }

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }
}
