using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Identity;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public RefreshTokenService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<IssuedRefreshToken> IssueAsync(string userId, CancellationToken cancellationToken = default)
    {
        var days = _configuration.GetValue<int?>("Jwt:RefreshDays") ?? 14;
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.AddDays(days);
        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            Revoked = false,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);
        return new IssuedRefreshToken(token, expiresAt);
    }

    public async Task<string?> GetUserIdIfValidAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var stored = await _context.RefreshTokens.FirstOrDefaultAsync(
            item => item.Token == token && !item.IsDeleted,
            cancellationToken);

        if (stored is null || stored.Revoked || stored.ExpiresAt <= DateTime.UtcNow)
            return null;

        stored.Revoked = true;
        stored.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        return stored.UserId;
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var stored = await _context.RefreshTokens.FirstOrDefaultAsync(
            item => item.Token == token && !item.IsDeleted,
            cancellationToken);
        if (stored is null)
            return;

        stored.Revoked = true;
        stored.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
