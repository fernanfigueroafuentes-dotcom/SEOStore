namespace SEOStore.Application.Interfaces.Services;

public sealed record IssuedRefreshToken(string Token, DateTime ExpiresAt);

public interface IRefreshTokenService
{
    Task<IssuedRefreshToken> IssueAsync(string userId, CancellationToken cancellationToken = default);

    Task<string?> GetUserIdIfValidAsync(string token, CancellationToken cancellationToken = default);

    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}
