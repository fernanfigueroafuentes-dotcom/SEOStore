using Microsoft.Extensions.Configuration;
using SEOStore.Application.Features.Settings.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Application.Services;

public class SettingService : ISettingService
{
    private readonly ISettingRepository _settingRepository;
    private readonly IConfiguration _configuration;

    public SettingService(ISettingRepository settingRepository, IConfiguration configuration)
    {
        _settingRepository = settingRepository;
        _configuration = configuration;
    }

    public async Task<SiteSettingsDto> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        return Map(await _settingRepository.GetCurrentAsync(cancellationToken), overlayConfig: true);
    }

    public async Task<SiteSettingsDto> GetEditableAsync(CancellationToken cancellationToken = default)
    {
        return Map(await _settingRepository.GetCurrentAsync(cancellationToken), overlayConfig: false);
    }

    public async Task UpdateAsync(SiteSettingsDto dto, CancellationToken cancellationToken = default)
    {
        var setting = await _settingRepository.GetForUpdateAsync(cancellationToken);
        if (setting is null)
        {
            setting = new SEOStore.Domain.Entities.Configuration.Setting { CreatedAt = DateTime.UtcNow };
            Apply(setting, dto);
            await _settingRepository.AddAsync(setting, cancellationToken);
            return;
        }

        Apply(setting, dto);
        await _settingRepository.UpdateAsync(setting, cancellationToken);
    }

    private SiteSettingsDto Map(SEOStore.Domain.Entities.Configuration.Setting? setting, bool overlayConfig)
    {
        string Pick(string? stored, params string?[] fallbacks) =>
            overlayConfig
                ? FirstNonEmpty(new[] { stored }.Concat(fallbacks).ToArray()) ?? string.Empty
                : stored?.Trim() ?? string.Empty;

        return new SiteSettingsDto
        {
            Id = setting?.Id ?? 0,
            SiteName = overlayConfig
                ? FirstNonEmpty(setting?.SiteName, _configuration["SITE_NAME"], _configuration["Site:SiteName"]) ?? "SEOStore"
                : (string.IsNullOrWhiteSpace(setting?.SiteName) ? "SEOStore" : setting.SiteName.Trim()),
            LogoUrl = Pick(setting?.LogoUrl),
            FaviconUrl = Pick(setting?.FaviconUrl),
            Phone = Pick(setting?.Phone),
            Email = Pick(setting?.Email),
            WhatsApp = overlayConfig
                ? FirstNonEmpty(setting?.WhatsApp, _configuration["SITE_WHATSAPP"], _configuration["Site:WhatsApp"]) ?? string.Empty
                : setting?.WhatsApp?.Trim() ?? string.Empty,
            Facebook = Pick(setting?.Facebook),
            Instagram = Pick(setting?.Instagram),
            Address = Pick(setting?.Address),
            PrimaryColor = overlayConfig
                ? FirstNonEmpty(setting?.PrimaryColor, _configuration["Site:PrimaryColor"]) ?? "#1a1a1a"
                : (string.IsNullOrWhiteSpace(setting?.PrimaryColor) ? "#1a1a1a" : setting.PrimaryColor.Trim()),
            SecondaryColor = overlayConfig
                ? FirstNonEmpty(setting?.SecondaryColor, _configuration["Site:SecondaryColor"]) ?? "#f6f6f6"
                : (string.IsNullOrWhiteSpace(setting?.SecondaryColor) ? "#f6f6f6" : setting.SecondaryColor.Trim()),
            GoogleAnalytics = overlayConfig
                ? FirstNonEmpty(setting?.GoogleAnalytics, _configuration["SITE_GA"], _configuration["Site:GoogleAnalytics"]) ?? string.Empty
                : setting?.GoogleAnalytics?.Trim() ?? string.Empty,
            GoogleTagManager = overlayConfig
                ? FirstNonEmpty(setting?.GoogleTagManager, _configuration["SITE_GTM"], _configuration["Site:GoogleTagManager"]) ?? string.Empty
                : setting?.GoogleTagManager?.Trim() ?? string.Empty,
            Currency = FirstNonEmpty(_configuration["SITE_CURRENCY"], _configuration["Site:Currency"]) ?? "ARS",
            SiteMode = (setting?.SiteMode ?? SEOStore.Domain.Entities.Configuration.SiteMode.Hybrid).ToString()
        };
    }

    private static void Apply(SEOStore.Domain.Entities.Configuration.Setting setting, SiteSettingsDto dto)
    {
        setting.SiteName = string.IsNullOrWhiteSpace(dto.SiteName) ? "SEOStore" : dto.SiteName.Trim();
        setting.LogoUrl = dto.LogoUrl?.Trim() ?? string.Empty;
        setting.FaviconUrl = dto.FaviconUrl?.Trim() ?? string.Empty;
        setting.Phone = dto.Phone?.Trim() ?? string.Empty;
        setting.Email = dto.Email?.Trim() ?? string.Empty;
        setting.WhatsApp = dto.WhatsApp?.Trim() ?? string.Empty;
        setting.Facebook = dto.Facebook?.Trim() ?? string.Empty;
        setting.Instagram = dto.Instagram?.Trim() ?? string.Empty;
        setting.Address = dto.Address?.Trim() ?? string.Empty;
        setting.PrimaryColor = string.IsNullOrWhiteSpace(dto.PrimaryColor) ? "#1a1a1a" : dto.PrimaryColor.Trim();
        setting.SecondaryColor = string.IsNullOrWhiteSpace(dto.SecondaryColor) ? "#f6f6f6" : dto.SecondaryColor.Trim();
        setting.GoogleAnalytics = dto.GoogleAnalytics?.Trim() ?? string.Empty;
        setting.GoogleTagManager = dto.GoogleTagManager?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(dto.SiteMode)
            && Enum.TryParse<SEOStore.Domain.Entities.Configuration.SiteMode>(dto.SiteMode, true, out var siteMode))
        {
            setting.SiteMode = siteMode;
        }
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value) && value != "#000000" && value != "#FFFFFF")
                return value.Trim();
        }

        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim();
    }
}
