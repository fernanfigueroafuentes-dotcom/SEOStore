using SEOStore.Application.Features.Settings.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface ISettingService
{
    Task<SiteSettingsDto> GetCurrentAsync(CancellationToken cancellationToken = default);

    Task<SiteSettingsDto> GetEditableAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(SiteSettingsDto dto, CancellationToken cancellationToken = default);
}
