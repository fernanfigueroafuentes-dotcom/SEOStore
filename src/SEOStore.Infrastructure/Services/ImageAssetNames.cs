using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Infrastructure.Services;

public static class ImageAssetNames
{
    public static string FromProduct(string productName, int sequence)
    {
        var slug = Product.GenerateSlug(productName);
        return sequence <= 1 ? slug : $"{slug}-{sequence:00}";
    }

    public static string FromFileName(string fileName, string? assetName = null)
    {
        if (!string.IsNullOrWhiteSpace(assetName))
            return Product.GenerateSlug(assetName);

        return Product.GenerateSlug(Path.GetFileNameWithoutExtension(fileName));
    }
}
