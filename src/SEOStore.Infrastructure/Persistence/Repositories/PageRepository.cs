using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Content;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class PageRepository : IPageRepository
{
    private readonly ApplicationDbContext _context;

    public PageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Page>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pages
            .AsNoTracking()
            .Where(page => !page.IsDeleted)
            .OrderBy(page => page.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Page>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pages
            .AsNoTracking()
            .Where(page => !page.IsDeleted && page.Published)
            .OrderBy(page => page.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<Page?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Pages.FirstOrDefaultAsync(page => page.Id == id && !page.IsDeleted, cancellationToken);
    }

    public async Task<Page?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Pages.FirstOrDefaultAsync(
            page => page.Slug == slug && !page.IsDeleted && page.Published,
            cancellationToken);
    }

    public async Task AddAsync(Page page, CancellationToken cancellationToken = default)
    {
        _context.Pages.Add(page);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Page page, CancellationToken cancellationToken = default)
    {
        _context.Pages.Update(page);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Page page, CancellationToken cancellationToken = default)
    {
        page.IsDeleted = true;
        page.Slug = $"{page.Slug}-deleted-{page.Id}";
        page.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(page, cancellationToken);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Pages.AnyAsync(
            page => page.Slug == slug && !page.IsDeleted && (excludeId == null || page.Id != excludeId),
            cancellationToken);
    }
}
