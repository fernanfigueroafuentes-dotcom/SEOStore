using SEOStore.Application.Features.Categories.DTOs;
using SEOStore.Application.Features.Products.DTOs;

namespace SEOStore.Web.Models.Store;

public class CategoryPageViewModel
{
    public CategoryDto Category { get; init; } = null!;

    public IReadOnlyList<ProductDto> Products { get; init; } = [];
}
