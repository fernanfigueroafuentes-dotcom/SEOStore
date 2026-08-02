using Microsoft.AspNetCore.Identity;

namespace SEOStore.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

}