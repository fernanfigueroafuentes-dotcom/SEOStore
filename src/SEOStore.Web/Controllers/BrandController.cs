using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Brands.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var brands = await _brandService.GetAllAsync(cancellationToken);
            return Ok(brands);
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
            var brand = await _brandService.GetByIdAsync(id, cancellationToken);
            return brand is null ? NotFound() : Ok(brand);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var brand = await _brandService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest();

            var brand = await _brandService.UpdateAsync(dto, cancellationToken);
            return Ok(brand);
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

    [AdminApi]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _brandService.DeleteAsync(id, cancellationToken);
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
