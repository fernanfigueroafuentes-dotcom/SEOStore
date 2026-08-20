using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Configuration;

public class Setting : BaseEntity
{
    public string SiteName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string FaviconUrl { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#000000";
    public string SecondaryColor { get; set; } = "#FFFFFF";
    public string GoogleAnalytics { get; set; } = string.Empty;
    public string GoogleTagManager { get; set; } = string.Empty;

    public SiteMode SiteMode { get; set; } = SiteMode.Hybrid;
}