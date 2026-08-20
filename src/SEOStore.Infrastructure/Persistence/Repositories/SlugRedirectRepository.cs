using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Seo;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class SlugRedirectRepository : ISlugRedirectRepository
{
    private readonly ApplicationDbContext _context;

    public SlugRedirectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<SlugRedirect?> GetByOldPathAsync(string oldPath, CancellationToken cancellationToken = default)
    {
        return _context.SlugRedirects.FirstOrDefaultAsync(
            redirect => redirect.OldPath == oldPath && !redirect.IsDeleted,
            cancellationToken);
    }

    public Task<List<SlugRedirect>> GetByNewPathAsync(string newPath, CancellationToken cancellationToken = default)
    {
        return _context.SlugRedirects
            .Where(redirect => redirect.NewPath == newPath && !redirect.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SlugRedirect redirect, CancellationToken cancellationToken = default)
    {
        _context.SlugRedirects.Add(redirect);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SlugRedirect redirect, CancellationToken cancellationToken = default)
    {
        _context.SlugRedirects.Update(redirect);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SlugRedirect redirect, CancellationToken cancellationToken = default)
    {
        redirect.IsDeleted = true;
        redirect.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(redirect, cancellationToken);
    }
}
