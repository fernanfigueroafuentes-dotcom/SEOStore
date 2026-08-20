using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SEOStore.Web.Models.Auth;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    [DefaultValue("user@example.com")]
    public string Email { get; init; } = string.Empty;

    [Required]
    [DefaultValue("JwtPass123")]
    public string Password { get; init; } = string.Empty;
}
