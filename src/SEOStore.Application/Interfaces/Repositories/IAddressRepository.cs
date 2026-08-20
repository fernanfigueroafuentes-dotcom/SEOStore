using SEOStore.Domain.Entities.Users;

namespace SEOStore.Application.Interfaces.Repositories;

public interface IAddressRepository
{
    Task<List<Address>> ListByUserAsync(string userId, CancellationToken cancellationToken = default);

    Task<Address?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default);

    Task AddAsync(Address address, CancellationToken cancellationToken = default);

    Task UpdateAsync(Address address, CancellationToken cancellationToken = default);

    Task DeleteAsync(Address address, CancellationToken cancellationToken = default);
}
