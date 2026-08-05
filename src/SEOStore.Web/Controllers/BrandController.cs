using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Brands.DTOs;
using SEOStore.Application.Interfaces.Services;

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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var brand = await _brandService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

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
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _brandService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}