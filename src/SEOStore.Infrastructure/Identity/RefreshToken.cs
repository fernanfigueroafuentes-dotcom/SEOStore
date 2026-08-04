using SEOStore.Domain.Common;

namespace SEOStore.Infrastructure.Identity;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public string Token { get; set; } = string.Empty;


    public DateTime ExpiresAt { get; set; }


    public bool Revoked { get; set; }
}