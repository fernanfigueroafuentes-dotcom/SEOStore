using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SEOStore.Web.Models.Auth;

public sealed class RegisterRequest
{
    [Required]
    [EmailAddress]
    [DefaultValue("user@example.com")]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter and one digit.")]
    [DefaultValue("JwtPass123")]
    public string Password { get; init; } = string.Empty;

    [DefaultValue("Fer")]
    public string? FirstName { get; init; }

    [DefaultValue("Figueroa")]
    public string? LastName { get; init; }
}
