namespace SEOStore.Application.Features.Settings.DTOs;

public class SiteSettingsDto
{
    public int Id { get; set; }

    public string SiteName { get; set; } = "SEOStore";

    public string LogoUrl { get; set; } = string.Empty;

    public string FaviconUrl { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string WhatsApp { get; set; } = string.Empty;

    public string Facebook { get; set; } = string.Empty;

    public string Instagram { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string PrimaryColor { get; set; } = "#1a1a1a";

    public string SecondaryColor { get; set; } = "#f6f6f6";

    public string GoogleAnalytics { get; set; } = string.Empty;

    public string GoogleTagManager { get; set; } = string.Empty;

    public string Currency { get; set; } = "ARS";

    public string? SiteMode { get; set; }

    public bool CheckoutEnabled =>
        !string.Equals(SiteMode, "Catalog", StringComparison.OrdinalIgnoreCase);

    public string WhatsAppDigits => new string(WhatsApp.Where(char.IsDigit).ToArray());
}
