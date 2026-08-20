using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Orders.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IOrderService _orderService;

    public PaymentsController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [AdminApi]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePaymentStatusDto dto, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _orderService.UpdatePaymentStatusAsync(id, dto, cancellationToken));
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
