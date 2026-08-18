using SEOStore.Application.DTOs.Cart;
using SEOStore.Application.Interfaces;
using SEOStore.Domain.Entities.Commerce;
using SEOStore.Application.Interfaces.Repositories;

namespace SEOStore.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartDto> GetCartAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart is null)
        {
            return new CartDto
            {
                UserId = userId,
                Items = new List<CartItemDto>()
            };
        }

        return MapToDto(cart);
    }

    public async Task<CartDto> AddItemAsync(
        string userId,
        AddCartItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "UserId cannot be empty.",
                nameof(userId));

        if (dto.Quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(dto.Quantity));

        var product = await _productRepository.GetByIdAsync(
            dto.ProductId,
            cancellationToken);

        if (product is null)
            throw new KeyNotFoundException(
                "Product not found.");

        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart is null)
        {
            cart = new Cart(userId);

            await _cartRepository.AddAsync(
                cart,
                cancellationToken);
        }

        cart.AddItem(
            product.Id,
            dto.Quantity,
            product.Price);

        await _cartRepository.UpdateAsync(
            cart,
            cancellationToken);

        return MapToDto(cart);
    }

    public async Task<CartDto> UpdateItemQuantityAsync(
        string userId,
        int productId,
        UpdateCartItemDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(dto.Quantity));

        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart is null)
            throw new KeyNotFoundException(
                "Cart not found.");

        cart.UpdateItemQuantity(
            productId,
            dto.Quantity);

        await _cartRepository.UpdateAsync(
            cart,
            cancellationToken);

        return MapToDto(cart);
    }

    public async Task RemoveItemAsync(
        string userId,
        int productId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart is null)
            throw new KeyNotFoundException(
                "Cart not found.");

        cart.RemoveItem(productId);

        await _cartRepository.UpdateAsync(
            cart,
            cancellationToken);
    }

    public async Task ClearCartAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(
            userId,
            cancellationToken);

        if (cart is null)
            return;

        cart.Clear();

        await _cartRepository.SaveChangesAsync(
            cancellationToken);
    }

    private static CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.Items.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}