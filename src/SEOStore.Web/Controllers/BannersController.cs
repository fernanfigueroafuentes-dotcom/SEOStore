using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Banners.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/banners")]
public class BannersController : ControllerBase
{
    private readonly IBannerService _bannerService;

    public BannersController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _bannerService.GetActiveAsync(cancellationToken));
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _bannerService.GetAllAsync(cancellationToken));
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var banner = await _bannerService.GetByIdAsync(id, cancellationToken);
            return banner is null ? NotFound() : Ok(banner);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertBannerDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var banner = await _bannerService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = banner.Id }, banner);
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { message = "The request cannot be completed." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertBannerDto dto, CancellationToken cancellationToken)
    {
        try
        {
            dto.Id = id;
            return Ok(await _bannerService.UpdateAsync(dto, cancellationToken));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "The requested resource was not found." });
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { message = "The request cannot be completed." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _bannerService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "The requested resource was not found." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
}
