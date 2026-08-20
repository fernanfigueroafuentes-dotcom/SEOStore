using System.Net;
using System.Text.RegularExpressions;
using Ganss.Xss;
using Microsoft.AspNetCore.Html;

namespace SEOStore.Web.Html;

public static class ProductDescriptionHtml
{
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    public static string Sanitize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var html = LooksLikeHtml(value)
            ? value
            : $"<p>{WebUtility.HtmlEncode(value).Replace("\r\n", "\n").Replace("\n", "<br />")}</p>";

        return Sanitizer.Sanitize(html).Trim();
    }

    public static IHtmlContent ToHtml(string? value) =>
        new HtmlString(Sanitize(value));

    private static bool LooksLikeHtml(string value) =>
        Regex.IsMatch(value, @"<[a-zA-Z][\s\S]*>");

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        foreach (var tag in new[] { "p", "br", "strong", "b", "em", "i", "u", "s", "ul", "ol", "li", "h2", "h3", "h4", "a", "span", "blockquote" })
            sanitizer.AllowedTags.Add(tag);

        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("href");
        sanitizer.AllowedAttributes.Add("target");
        sanitizer.AllowedAttributes.Add("rel");
        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        sanitizer.AllowedSchemes.Add("mailto");
        return sanitizer;
    }
}
