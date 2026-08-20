using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Content;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class BannerRepository : IBannerRepository
{
    private readonly ApplicationDbContext _context;

    public BannerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Banner>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _context.Banners
            .AsNoTracking()
            .Where(banner => !banner.IsDeleted)
            .OrderBy(banner => banner.DisplayOrder)
            .ThenBy(banner => banner.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Banner>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return _context.Banners
            .AsNoTracking()
            .Where(banner => !banner.IsDeleted && banner.Active)
            .OrderBy(banner => banner.DisplayOrder)
            .ThenBy(banner => banner.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Banner?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Banners.FirstOrDefaultAsync(
            banner => banner.Id == id && !banner.IsDeleted,
            cancellationToken);
    }

    public async Task AddAsync(Banner banner, CancellationToken cancellationToken = default)
    {
        _context.Banners.Add(banner);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Banner banner, CancellationToken cancellationToken = default)
    {
        _context.Banners.Update(banner);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Banner banner, CancellationToken cancellationToken = default)
    {
        banner.IsDeleted = true;
        banner.UpdatedAt = DateTime.UtcNow;
        await UpdateAsync(banner, cancellationToken);
    }
}
