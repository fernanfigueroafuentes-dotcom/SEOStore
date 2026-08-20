using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Settings.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsApiController : ControllerBase
{
    private readonly ISettingService _settingService;

    public SettingsApiController(ISettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _settingService.GetCurrentAsync(cancellationToken));
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SiteSettingsDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _settingService.UpdateAsync(dto, cancellationToken);
            return Ok(await _settingService.GetEditableAsync(cancellationToken));
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}
