namespace SEOStore.Application.Common;

public enum SlugKind
{
    Product,
    Category,
    Page,
    BlogPost
}

public static class PublicSlug
{
    public static string Path(SlugKind kind, string slug) => kind switch
    {
        SlugKind.Product => $"/producto/{slug}",
        SlugKind.Category => $"/categoria/{slug}",
        SlugKind.Page => $"/pagina/{slug}",
        SlugKind.BlogPost => $"/blog/{slug}",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    public static bool TryParse(string path, out SlugKind kind, out string slug)
    {
        kind = default;
        slug = string.Empty;
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var value = path.Trim();
        if (value.StartsWith("/producto/", StringComparison.OrdinalIgnoreCase))
        {
            kind = SlugKind.Product;
            slug = value["/producto/".Length..];
        }
        else if (value.StartsWith("/categoria/", StringComparison.OrdinalIgnoreCase))
        {
            kind = SlugKind.Category;
            slug = value["/categoria/".Length..];
        }
        else if (value.StartsWith("/pagina/", StringComparison.OrdinalIgnoreCase))
        {
            kind = SlugKind.Page;
            slug = value["/pagina/".Length..];
        }
        else if (value.StartsWith("/blog/", StringComparison.OrdinalIgnoreCase))
        {
            kind = SlugKind.BlogPost;
            slug = value["/blog/".Length..];
        }
        else
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(slug) && !slug.Contains('/');
    }
}
