using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Carts.DTOs;
using SEOStore.Application.Interfaces.Services;

namespace SEOStore.Web.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public Task<IActionResult> Get(CancellationToken cancellationToken) =>
        ExecuteAsync(userId => _cartService.GetCartAsync(userId, cancellationToken));

    [HttpPost("items")]
    public Task<IActionResult> AddItem([FromBody] AddCartItemDto dto, CancellationToken cancellationToken) =>
        ExecuteAsync(userId => _cartService.AddItemAsync(userId, dto, cancellationToken));

    [HttpPut("items/{productId:int}")]
    public Task<IActionResult> UpdateItem(int productId, [FromBody] UpdateCartItemDto dto, CancellationToken cancellationToken) =>
        ExecuteAsync(userId => _cartService.UpdateItemQuantityAsync(userId, productId, dto, cancellationToken));

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            await _cartService.RemoveItemAsync(userId, productId, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest(new { message = "The request is invalid." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "The requested resource was not found." });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        await _cartService.ClearCartAsync(userId, cancellationToken);
        return NoContent();
    }

    private async Task<IActionResult> ExecuteAsync(Func<string, Task<CartDto>> action)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        try
        {
            return Ok(await action(userId));
        }
        catch (ArgumentException)
        {
            return BadRequest(new { message = "The request is invalid." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "The requested resource was not found." });
        }
    }

    private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
