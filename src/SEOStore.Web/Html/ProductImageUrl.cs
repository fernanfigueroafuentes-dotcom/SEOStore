namespace SEOStore.Web.Html;

public static class ProductImageUrl
{
    public const int CardWidth = 400;
    public const int CardHeight = 500;

    public const int HeroWidth = 640;
    public const int HeroHeight = 800;

    public const int ThumbWidth = 240;
    public const int ThumbHeight = 300;

    public static string Card(string? url) => Transform(url, CardWidth, CardHeight);

    public static string Hero(string? url) => Transform(url, HeroWidth, HeroHeight);

    public static string Thumb(string? url) => Transform(url, ThumbWidth, ThumbHeight);

    private static string Transform(string? url, int width, int height)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        const string marker = "/image/upload/";
        var index = url.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
            return url;

        var insertAt = index + marker.Length;
        if (url.IndexOf($"w_{width}", insertAt, StringComparison.OrdinalIgnoreCase) >= 0)
            return url;

        return url.Insert(insertAt, $"f_auto,q_auto,c_limit,w_{width},h_{height}/");
    }
}
