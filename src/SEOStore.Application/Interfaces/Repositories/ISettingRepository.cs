using SEOStore.Domain.Entities.Configuration;

namespace SEOStore.Application.Interfaces.Repositories;

public interface ISettingRepository
{
    Task<Setting?> GetCurrentAsync(CancellationToken cancellationToken = default);

    Task<Setting?> GetForUpdateAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Setting setting, CancellationToken cancellationToken = default);

    Task UpdateAsync(Setting setting, CancellationToken cancellationToken = default);
}
