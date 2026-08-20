using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Content.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/pages")]
public class PagesController : ControllerBase
{
    private readonly IPageService _pageService;

    public PagesController(IPageService pageService)
    {
        _pageService = pageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _pageService.GetAllAsync(cancellationToken));
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
            var page = await _pageService.GetByIdAsync(id, cancellationToken);
            return page is null ? NotFound() : Ok(page);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var page = await _pageService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = page.Id }, page);
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
    public async Task<IActionResult> Update(int id, [FromBody] UpsertPageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            dto.Id = id;
            var page = await _pageService.UpdateAsync(dto, cancellationToken);
            return Ok(page);
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
            await _pageService.DeleteAsync(id, cancellationToken);
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
