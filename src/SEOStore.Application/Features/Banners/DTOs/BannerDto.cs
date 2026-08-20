namespace SEOStore.Application.Features.Banners.DTOs;

public class BannerDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? Link { get; set; }

    public int DisplayOrder { get; set; }

    public bool Active { get; set; }
}
