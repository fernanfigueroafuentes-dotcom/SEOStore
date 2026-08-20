namespace SEOStore.Application.Features.Content.DTOs;

public class UpsertPageDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Slug { get; set; }

    public string Content { get; set; } = string.Empty;

    public bool Published { get; set; } = true;

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public bool Index { get; set; } = true;

    public bool Follow { get; set; } = true;
}
