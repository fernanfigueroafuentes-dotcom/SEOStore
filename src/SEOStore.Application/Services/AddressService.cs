using SEOStore.Application.Features.Addresses.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Users;

namespace SEOStore.Application.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;

    public AddressService(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task<IEnumerable<AddressDto>> GetMineAsync(string userId, CancellationToken cancellationToken = default)
    {
        var addresses = await _addressRepository.ListByUserAsync(userId, cancellationToken);
        return addresses.Select(Map);
    }

    public async Task<AddressDto> CreateAsync(string userId, UpsertAddressDto dto, CancellationToken cancellationToken = default)
    {
        Require(dto);
        var address = new Address
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        Apply(address, dto);
        if (dto.IsDefault)
            await ClearDefaultsAsync(userId, excludeId: null, cancellationToken);

        await _addressRepository.AddAsync(address, cancellationToken);
        return Map(address);
    }

    public async Task<AddressDto> UpdateAsync(string userId, int id, UpsertAddressDto dto, CancellationToken cancellationToken = default)
    {
        Require(dto);
        var address = await _addressRepository.GetByIdAsync(id, userId, cancellationToken)
            ?? throw new KeyNotFoundException("The address was not found.");

        Apply(address, dto);
        address.UpdatedAt = DateTime.UtcNow;
        if (dto.IsDefault)
            await ClearDefaultsAsync(userId, address.Id, cancellationToken);

        await _addressRepository.UpdateAsync(address, cancellationToken);
        return Map(address);
    }

    public async Task DeleteAsync(string userId, int id, CancellationToken cancellationToken = default)
    {
        var address = await _addressRepository.GetByIdAsync(id, userId, cancellationToken)
            ?? throw new KeyNotFoundException("The address was not found.");
        await _addressRepository.DeleteAsync(address, cancellationToken);
    }

    private async Task ClearDefaultsAsync(string userId, int? excludeId, CancellationToken cancellationToken)
    {
        var addresses = await _addressRepository.ListByUserAsync(userId, cancellationToken);
        foreach (var address in addresses.Where(item => item.IsDefault && item.Id != excludeId))
        {
            address.IsDefault = false;
            address.UpdatedAt = DateTime.UtcNow;
            await _addressRepository.UpdateAsync(address, cancellationToken);
        }
    }

    private static void Apply(Address address, UpsertAddressDto dto)
    {
        address.Street = dto.Street.Trim();
        address.City = dto.City.Trim();
        address.Region = dto.Region.Trim();
        address.PostalCode = dto.PostalCode.Trim();
        address.IsDefault = dto.IsDefault;
    }

    private static void Require(UpsertAddressDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Street) ||
            string.IsNullOrWhiteSpace(dto.City) ||
            string.IsNullOrWhiteSpace(dto.Region) ||
            string.IsNullOrWhiteSpace(dto.PostalCode))
        {
            throw new ArgumentException("Street, city, region and postal code are required.");
        }
    }

    private static AddressDto Map(Address address) => new()
    {
        Id = address.Id,
        Street = address.Street,
        City = address.City,
        Region = address.Region,
        PostalCode = address.PostalCode,
        IsDefault = address.IsDefault
    };
}
