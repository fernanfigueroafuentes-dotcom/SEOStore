using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/products/{productId:int}/images")]
public class ProductImageController : ControllerBase
{
    private readonly IProductImageService _productImageService;
    private readonly IImageStorageService _imageStorageService;

    public ProductImageController(
        IProductImageService productImageService,
        IImageStorageService imageStorageService)
    {
        _productImageService = productImageService;
        _imageStorageService = imageStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int productId, CancellationToken cancellationToken)
    {
        try
        {
            var images = await _productImageService.GetAllByProductAsync(productId, cancellationToken);
            return Ok(images);
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int productId, int id, CancellationToken cancellationToken)
    {
        try
        {
            var image = await _productImageService.GetByIdAsync(id, cancellationToken);
            if (image is null || image.ProductId != productId)
                return NotFound();

            return Ok(image);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(int productId, [FromBody] CreateProductImageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (dto.ProductId != 0 && dto.ProductId != productId)
                return BadRequest(new { message = "ProductId does not match route parameter." });

            dto.ProductId = productId;
            var image = await _productImageService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { productId, id = image.Id }, image);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(int productId, IFormFile file, [FromForm] string? alt = null, [FromForm] bool isPrimary = false, [FromForm] int displayOrder = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { message = "A file is required for upload." });

            var folder = $"products/{productId}";
            using var stream = file.OpenReadStream();
            var uploadResult = await _imageStorageService.UploadAsync(stream, file.FileName, folder, cancellationToken);

            var dto = new CreateProductImageDto
            {
                ProductId = productId,
                Url = uploadResult.Url,
                PublicId = uploadResult.PublicId,
                Alt = alt,
                IsPrimary = isPrimary,
                DisplayOrder = displayOrder
            };

            var image = await _productImageService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { productId, id = image.Id }, image);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int productId, int id, [FromBody] UpdateProductImageDto dto, CancellationToken cancellationToken)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest();

            if (dto.ProductId != 0 && dto.ProductId != productId)
                return BadRequest(new { message = "ProductId does not match route parameter." });

            dto.ProductId = productId;
            var image = await _productImageService.UpdateAsync(dto, cancellationToken);
            return Ok(image);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int productId, int id, CancellationToken cancellationToken)
    {
        try
        {
            var image = await _productImageService.GetByIdAsync(id, cancellationToken);
            if (image is null || image.ProductId != productId)
                return NotFound();

            await _productImageService.DeleteAsync(id, cancellationToken);
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

    [HttpPatch("{id:int}/primary")]
    public async Task<IActionResult> SetPrimary(int productId, int id, CancellationToken cancellationToken)
    {
        try
        {
            var image = await _productImageService.GetByIdAsync(id, cancellationToken);
            if (image is null || image.ProductId != productId)
                return NotFound();

            var updated = await _productImageService.SetPrimaryAsync(id, cancellationToken);
            return Ok(updated);
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
