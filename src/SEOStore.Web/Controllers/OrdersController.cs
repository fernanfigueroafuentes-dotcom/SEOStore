using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Orders.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto, CancellationToken cancellationToken)
    {
        var userId = ApiUser.Id(User);
        if (userId is null)
            return Unauthorized();

        try
        {
            var order = await _orderService.CheckoutAsync(userId, dto ?? new CheckoutDto(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (ArgumentException)
        {
            return BadRequest(new { message = "The request is invalid." });
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

    [HttpGet]
    public async Task<IActionResult> Mine(CancellationToken cancellationToken)
    {
        var userId = ApiUser.Id(User);
        if (userId is null)
            return Unauthorized();

        return Ok(await _orderService.GetMineAsync(userId, cancellationToken));
    }

    [AdminApi]
    [HttpGet("all")]
    public async Task<IActionResult> All(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _orderService.GetAllAsync(cancellationToken));
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = ApiUser.Id(User);
        if (userId is null)
            return Unauthorized();

        var order = await _orderService.GetByIdAsync(id, userId, ApiUser.IsAdmin(User), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [AdminApi]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] UpdateOrderStatusDto dto, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _orderService.ChangeStatusAsync(id, dto, cancellationToken));
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

    [HttpPost("{id:int}/payments")]
    public async Task<IActionResult> AddPayment(int id, [FromBody] CreatePaymentDto dto, CancellationToken cancellationToken)
    {
        var userId = ApiUser.Id(User);
        if (userId is null)
            return Unauthorized();

        try
        {
            return Ok(await _orderService.AddPaymentAsync(id, userId, ApiUser.IsAdmin(User), dto, cancellationToken));
        }
        catch (ArgumentException)
        {
            return BadRequest(new { message = "The request is invalid." });
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
}
