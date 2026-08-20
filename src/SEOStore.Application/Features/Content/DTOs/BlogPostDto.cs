namespace SEOStore.Application.Features.Content.DTOs;

public class BlogPostDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? FeaturedImageUrl { get; set; }

    public bool Published { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string? Author { get; set; }

    public int DisplayOrder { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? CanonicalUrl { get; set; }

    public string? OgTitle { get; set; }

    public string? OgDescription { get; set; }

    public string? OgImage { get; set; }

    public bool Index { get; set; } = true;

    public bool Follow { get; set; } = true;
}
