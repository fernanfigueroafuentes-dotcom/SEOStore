using SEOStore.Domain.Common;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Domain.Entities.Commerce;

public class CartItem : BaseEntity
{
    public int CartId { get; private set; }

    public Cart Cart { get; private set; } = null!;

    public int ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    private CartItem()
    {
    }

    internal CartItem(
        int cartId,
        int productId,
        int quantity,
        decimal unitPrice)
    {
        if (cartId <= 0)
            throw new ArgumentException(
                "CartId must be greater than zero.",
                nameof(cartId));

        if (productId <= 0)
            throw new ArgumentException(
                "ProductId must be greater than zero.",
                nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException(
                "UnitPrice cannot be negative.",
                nameof(unitPrice));

        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CreatedAt = DateTime.UtcNow;
    }

    internal void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        Quantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void UpdateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ArgumentException(
                "UnitPrice cannot be negative.",
                nameof(unitPrice));

        UnitPrice = unitPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal GetTotal()
    {
        return Quantity * UnitPrice;
    }
}
