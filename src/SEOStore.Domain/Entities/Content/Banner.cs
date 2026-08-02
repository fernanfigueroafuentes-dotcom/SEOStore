using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Content;
public class Banner : BaseEntity 
{
    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? Link { get; set; }

    public int DisplayOrder { get; set; }

    public bool Active { get; set; }
}