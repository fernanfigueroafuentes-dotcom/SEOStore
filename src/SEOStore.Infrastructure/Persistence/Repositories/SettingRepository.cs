using Microsoft.EntityFrameworkCore;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Domain.Entities.Configuration;
using SEOStore.Infrastructure.Persistence;

namespace SEOStore.Infrastructure.Persistence.Repositories;

public class SettingRepository : ISettingRepository
{
    private readonly ApplicationDbContext _context;

    public SettingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Setting?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Settings
            .AsNoTracking()
            .OrderBy(setting => setting.Id)
            .FirstOrDefaultAsync(setting => !setting.IsDeleted, cancellationToken);
    }

    public async Task<Setting?> GetForUpdateAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Settings
            .OrderBy(setting => setting.Id)
            .FirstOrDefaultAsync(setting => !setting.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Setting setting, CancellationToken cancellationToken = default)
    {
        _context.Settings.Add(setting);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Setting setting, CancellationToken cancellationToken = default)
    {
        setting.UpdatedAt = DateTime.UtcNow;
        _context.Settings.Update(setting);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
