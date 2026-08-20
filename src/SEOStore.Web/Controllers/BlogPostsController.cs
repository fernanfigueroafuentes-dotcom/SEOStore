using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Content.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/blog")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostsController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _blogPostService.GetAllAsync(cancellationToken));
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
            var post = await _blogPostService.GetByIdAsync(id, cancellationToken);
            return post is null ? NotFound() : Ok(post);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [AdminApi]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertBlogPostDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var post = await _blogPostService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
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
    public async Task<IActionResult> Update(int id, [FromBody] UpsertBlogPostDto dto, CancellationToken cancellationToken)
    {
        try
        {
            dto.Id = id;
            var post = await _blogPostService.UpdateAsync(dto, cancellationToken);
            return Ok(post);
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
            await _blogPostService.DeleteAsync(id, cancellationToken);
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
