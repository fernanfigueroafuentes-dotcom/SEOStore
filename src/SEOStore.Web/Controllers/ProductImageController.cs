using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Products.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

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
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "The requested resource was not found." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
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
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
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
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(int productId, IFormFile file, [FromForm] string? alt = null, [FromForm] bool isPrimary = false, [FromForm] int displayOrder = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { message = "A file is required for upload." });

            var folder = $"products/{productId}";
            using var stream = file.OpenReadStream();
            var uploadResult = await _imageStorageService.UploadAsync(
                stream,
                file.FileName,
                folder,
                assetName: null,
                cancellationToken);

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
