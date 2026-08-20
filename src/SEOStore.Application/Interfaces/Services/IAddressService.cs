using SEOStore.Application.Features.Addresses.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IAddressService
{
    Task<IEnumerable<AddressDto>> GetMineAsync(string userId, CancellationToken cancellationToken = default);

    Task<AddressDto> CreateAsync(string userId, UpsertAddressDto dto, CancellationToken cancellationToken = default);

    Task<AddressDto> UpdateAsync(string userId, int id, UpsertAddressDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(string userId, int id, CancellationToken cancellationToken = default);
}
