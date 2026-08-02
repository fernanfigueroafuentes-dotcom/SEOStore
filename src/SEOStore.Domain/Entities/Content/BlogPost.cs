using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Content;

public class BlogPost : SeoEntity
{
    // Información principal
    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    // Imagen destacada
    public string? FeaturedImageUrl { get; set; }

    // Estado
    public bool Published { get; set; } = false;

    public DateTime? PublishedAt { get; set; }

    // Autor
    public string? Author { get; set; }

    // Organización
    public int DisplayOrder { get; set; }

    // Métricas
    public int ViewCount { get; set; }
}