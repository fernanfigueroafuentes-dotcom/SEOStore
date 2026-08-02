using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Catalog;

public class ProductImage : BaseEntity
{
    public string Url { get; set; } = string.Empty;

    public string PublicId { get; set; } = string.Empty;

    public string? Alt { get; set; }

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
}