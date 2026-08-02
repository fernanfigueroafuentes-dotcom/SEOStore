using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Integrations;

public class MercadoLibreAuth : BaseEntity
{
    public long UserId { get; set; }

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public int ExpiresIn { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime AuthorizedAt { get; set; } = DateTime.UtcNow;
}