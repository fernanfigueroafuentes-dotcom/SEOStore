namespace SEOStore.Application.Features.Categories.DTOs;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? ParentCategoryId { get; set; }

    public int DisplayOrder { get; set; }

    public bool Published { get; set; } = true;

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public bool Index { get; set; } = true;

    public bool Follow { get; set; } = true;
}
