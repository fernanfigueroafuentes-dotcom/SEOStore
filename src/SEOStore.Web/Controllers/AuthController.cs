using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Infrastructure.Identity;
using SEOStore.Web.Models.Auth;

namespace SEOStore.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IRefreshTokenService refreshTokenService,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var email = request.Email.Trim();
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return BadRequest(new { message = "This email is already registered." });

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Registration failed for {Email} with codes {Codes}",
                email,
                string.Join(",", result.Errors.Select(error => error.Code)));

            if (result.Errors.Any(error => error.Code is "DuplicateUserName" or "DuplicateEmail"))
                return BadRequest(new { message = "This email is already registered." });

            return BadRequest(new
            {
                message = "The password does not meet the security requirements.",
                errors = result.Errors
                    .Select(MapIdentityError)
                    .Distinct()
                    .ToArray()
            });
        }

        return Ok(await CreateToken(user));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(await CreateToken(user));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        var userId = await _refreshTokenService.GetUserIdIfValidAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
            return Unauthorized(new { message = "Invalid credentials." });

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(await CreateToken(user));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeAsync(request.RefreshToken, cancellationToken);
        return NoContent();
    }

    private async Task<object> CreateToken(ApplicationUser user)
    {
        var key = _configuration["Jwt:Key"]!;
        var issuer = _configuration["Jwt:Issuer"]!;
        var audience = _configuration["Jwt:Audience"]!;
        var expirationMinutes = _configuration.GetValue<int?>("Jwt:ExpirationMinutes") ?? 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };
        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim("role", role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var refresh = await _refreshTokenService.IssueAsync(user.Id);

        return new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            tokenType = "Bearer",
            expiresAt,
            refreshToken = refresh.Token,
            refreshExpiresAt = refresh.ExpiresAt
        };
    }

    private static string MapIdentityError(IdentityError error) => error.Code switch
    {
        "PasswordTooShort" => "Password must be at least 8 characters.",
        "PasswordRequiresDigit" => "Password must contain at least one digit.",
        "PasswordRequiresUpper" => "Password must contain at least one uppercase letter.",
        "PasswordRequiresLower" => "Password must contain at least one lowercase letter.",
        "PasswordRequiresNonAlphanumeric" => "Password must contain at least one special character.",
        "InvalidEmail" => "Email is invalid.",
        _ => "Registration could not be completed."
    };
}
