using System.Text.RegularExpressions;

namespace SEOStore.Application.Common;

public static class ContentSlug
{
    public static string From(string? value, string fallback = "item")
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        var slug = value.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? fallback : slug;
    }
}
