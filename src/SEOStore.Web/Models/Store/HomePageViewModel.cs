using SEOStore.Application.Features.Banners.DTOs;
using SEOStore.Application.Features.Categories.DTOs;
using SEOStore.Application.Features.Products.DTOs;

namespace SEOStore.Web.Models.Store;

public class HomePageViewModel
{
    public IReadOnlyList<BannerDto> Banners { get; init; } = [];

    public IReadOnlyList<CategoryDto> Categories { get; init; } = [];

    public IReadOnlyList<ProductDto> Featured { get; init; } = [];
}
