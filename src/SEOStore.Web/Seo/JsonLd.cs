using System.Text.Encodings.Web;
using System.Text.Json;
using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Features.Settings.DTOs;

namespace SEOStore.Web.Seo;

public static class JsonLd
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Organization(SiteSettingsDto site, string origin)
    {
        var data = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Organization",
            ["name"] = site.SiteName,
            ["url"] = origin
        };

        if (!string.IsNullOrWhiteSpace(site.LogoUrl))
            data["logo"] = site.LogoUrl;

        if (!string.IsNullOrWhiteSpace(site.Email))
            data["email"] = site.Email;

        if (!string.IsNullOrWhiteSpace(site.Phone))
            data["telephone"] = site.Phone;

        if (!string.IsNullOrWhiteSpace(site.Address))
            data["address"] = new Dictionary<string, object?>
            {
                ["@type"] = "PostalAddress",
                ["streetAddress"] = site.Address
            };

        return JsonSerializer.Serialize(data, Options);
    }

    public static string Product(ProductDto product, IEnumerable<string> images, string canonical, string currency)
    {
        var data = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Product",
            ["name"] = product.Name,
            ["sku"] = product.SKU,
            ["url"] = canonical
        };

        var description = product.ShortDescription ?? product.MetaDescription;
        if (!string.IsNullOrWhiteSpace(description))
            data["description"] = description;

        var imageList = images.Where(image => !string.IsNullOrWhiteSpace(image)).ToArray();
        if (imageList.Length > 0)
            data["image"] = imageList;

        if (product.ShowPrice)
        {
            data["offers"] = new Dictionary<string, object?>
            {
                ["@type"] = "Offer",
                ["url"] = canonical,
                ["price"] = product.Price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                ["priceCurrency"] = string.IsNullOrWhiteSpace(currency) ? "ARS" : currency,
                ["availability"] = "https://schema.org/InStock"
            };
        }

        return JsonSerializer.Serialize(data, Options);
    }

    public static string WebPage(string name, string url, string? description)
    {
        var data = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "WebPage",
            ["name"] = name,
            ["url"] = url
        };
        if (!string.IsNullOrWhiteSpace(description))
            data["description"] = description;
        return JsonSerializer.Serialize(data, Options);
    }

    public static string BlogPosting(string title, string url, string? summary, string? author, DateTime? publishedAt, string? image)
    {
        var data = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "BlogPosting",
            ["headline"] = title,
            ["url"] = url
        };
        if (!string.IsNullOrWhiteSpace(summary))
            data["description"] = summary;
        if (!string.IsNullOrWhiteSpace(author))
            data["author"] = new Dictionary<string, object?> { ["@type"] = "Person", ["name"] = author };
        if (publishedAt is not null)
            data["datePublished"] = publishedAt.Value.ToString("yyyy-MM-dd");
        if (!string.IsNullOrWhiteSpace(image))
            data["image"] = image;
        return JsonSerializer.Serialize(data, Options);
    }

    public static string Breadcrumb(IEnumerable<(string Name, string Url)> items)
    {
        var list = items.ToList();
        var data = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "BreadcrumbList",
            ["itemListElement"] = list.Select((item, index) => new Dictionary<string, object?>
            {
                ["@type"] = "ListItem",
                ["position"] = index + 1,
                ["name"] = item.Name,
                ["item"] = item.Url
            }).ToArray()
        };

        return JsonSerializer.Serialize(data, Options);
    }
}
