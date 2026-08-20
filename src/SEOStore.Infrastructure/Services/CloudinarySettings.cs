using Microsoft.Extensions.Configuration;

namespace SEOStore.Infrastructure.Services;

public static class CloudinarySettings
{
    public static bool TryRead(
        IConfiguration configuration,
        out string cloudName,
        out string apiKey,
        out string apiSecret)
    {
        cloudName = FirstValue(configuration, "CLOUDINARY_CLOUD_NAME", "Cloudinary:CloudName") ?? string.Empty;
        apiKey = FirstValue(configuration, "CLOUDINARY_API_KEY", "Cloudinary:ApiKey") ?? string.Empty;
        apiSecret = FirstValue(configuration, "CLOUDINARY_API_SECRET", "Cloudinary:ApiSecret") ?? string.Empty;

        var url = FirstValue(configuration, "CLOUDINARY_URL", "Cloudinary:Url");
        if (TryParseUrl(url, out var urlCloudName, out var urlKey, out var urlSecret))
        {
            if (string.IsNullOrWhiteSpace(cloudName))
                cloudName = urlCloudName;
            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = urlKey;
            if (string.IsNullOrWhiteSpace(apiSecret))
                apiSecret = urlSecret;
        }

        return !string.IsNullOrWhiteSpace(cloudName)
            && !string.IsNullOrWhiteSpace(apiKey)
            && !string.IsNullOrWhiteSpace(apiSecret);
    }

    private static bool TryParseUrl(string? url, out string cloudName, out string apiKey, out string apiSecret)
    {
        cloudName = string.Empty;
        apiKey = string.Empty;
        apiSecret = string.Empty;

        if (string.IsNullOrWhiteSpace(url) ||
            !url.StartsWith("cloudinary://", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var rest = url["cloudinary://".Length..];
        var at = rest.LastIndexOf('@');
        if (at <= 0)
            return false;

        var userInfo = rest[..at];
        var colon = userInfo.IndexOf(':');
        if (colon <= 0)
            return false;

        apiKey = Uri.UnescapeDataString(userInfo[..colon]);
        apiSecret = Uri.UnescapeDataString(userInfo[(colon + 1)..]);
        cloudName = rest[(at + 1)..].Trim().TrimEnd('/').Split('/')[0];

        return !string.IsNullOrWhiteSpace(cloudName)
            && !string.IsNullOrWhiteSpace(apiKey)
            && !string.IsNullOrWhiteSpace(apiSecret);
    }

    private static string? FirstValue(IConfiguration configuration, params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return null;
    }
}
