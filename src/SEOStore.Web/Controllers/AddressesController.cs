using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEOStore.Application.Features.Addresses.DTOs;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Web.Security;

namespace SEOStore.Web.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/addresses")]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var userId = ApiUser.Id(User);
        if (userId is null)
            return Unauthorized();

        return Ok(await _addressService.GetMineAsync(userId, cancellationToken));
    }

    [HttpPost]
    public Task<IActionResult> Create([FromBody] UpsertAddressDto dto, CancellationToken cancellationToken) =>
        ExecuteAsync(userId => _addressService.CreateAsync(userId, dto, cancellationToken));

    [HttpPut("{id:int}")]
    public Task<IActionResult> Update(int id, [FromBody] UpsertAddressDto dto, CancellationToken cancellationToken) =>
        ExecuteAsync(userId => _addressService.UpdateAsync(userId, id, dto, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = ApiUser.Id(User);
        if (userId is null)
            return Unauthorized();

        try
        {
            await _addressService.DeleteAsync(userId, id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "The requested resource was not found." });
        }
    }

    private async Task<IActionResult> ExecuteAsync(Func<string, Task<AddressDto>> action)
    {
        var userId = ApiUser.Id(User);
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
}
