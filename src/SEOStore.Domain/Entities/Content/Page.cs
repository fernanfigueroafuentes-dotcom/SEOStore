using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Content;

public class Page : SeoEntity
{
    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool Published { get; set; }

}