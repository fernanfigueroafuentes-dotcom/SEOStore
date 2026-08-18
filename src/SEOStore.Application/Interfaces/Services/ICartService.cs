using SEOStore.Application.DTOs.Cart;

namespace SEOStore.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<CartDto> AddItemAsync(
        string userId,
        AddCartItemDto dto,
        CancellationToken cancellationToken = default);

    Task<CartDto> UpdateItemQuantityAsync(
        string userId,
        int productId,
        UpdateCartItemDto dto,
        CancellationToken cancellationToken = default);

    Task RemoveItemAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken = default);

    Task ClearCartAsync(
        string userId,
        CancellationToken cancellationToken = default);
}