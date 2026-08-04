using Microsoft.AspNetCore.Identity;

namespace SEOStore.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}