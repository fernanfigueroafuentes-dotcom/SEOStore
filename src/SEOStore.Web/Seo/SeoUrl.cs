namespace SEOStore.Web.Seo;

public static class SeoUrl
{
    public static string Absolute(HttpRequest request, string? pathOrUrl)
    {
        if (string.IsNullOrWhiteSpace(pathOrUrl))
            return $"{request.Scheme}://{request.Host}";

        if (pathOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            pathOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return pathOrUrl;
        }

        var path = pathOrUrl.StartsWith('/') ? pathOrUrl : "/" + pathOrUrl;
        return $"{request.Scheme}://{request.Host}{path}";
    }
}
