namespace SEOStore.Web.Seo;

public sealed class SeoPage
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string CanonicalPath { get; init; } = "/";

    public string? OgTitle { get; init; }

    public string? OgDescription { get; init; }

    public string? OgImage { get; init; }

    public bool Index { get; init; } = true;

    public bool Follow { get; init; } = true;

    public IReadOnlyList<string> JsonLd { get; init; } = [];

    public static SeoPage Admin(string title, string path) => new()
    {
        Title = title,
        CanonicalPath = path,
        Index = false,
        Follow = false
    };
}
