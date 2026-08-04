namespace SEOStore.Application.Features.Categories.DTOs;

public class UpdateCategoryDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public int? ParentCategoryId { get; set; }

    public bool Published { get; set; }

    public int DisplayOrder { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }
}
