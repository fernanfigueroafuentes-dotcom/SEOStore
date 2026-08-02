namespace SEOStore.Domain.Common;

public abstract class SeoEntity : BaseEntity
{
    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? CanonicalUrl { get; set; }

    public string? OgTitle { get; set; }

    public string? OgDescription { get; set; }

    public string? OgImage { get; set; }


    public string? StructuredData { get; set; }


    public bool Index { get; set; } = true;

    public bool Follow { get; set; } = true;
}