using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Commerce;

public class Cart : BaseEntity
{
    public string UserId { get; private set; } = null!;

    public ICollection<CartItem> Items { get; private set; } = new List<CartItem>();

    private Cart()
    {
    }

    public Cart(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        UserId = userId;
    }

    public void AddItem(
        int productId,
        int quantity,
        decimal unitPrice)
    {
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

        var existingItem = Items.FirstOrDefault(
            x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
            UpdatedAt = DateTime.UtcNow;
            return;
        }

        Items.Add(
            new CartItem(
                Id,
                productId,
                quantity,
                unitPrice));

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItemQuantity(int productId, int quantity)
    {
        var item = Items.FirstOrDefault(
            x => x.ProductId == productId);

        if (item is null)
            throw new InvalidOperationException(
                "Product is not in the cart.");

        if (quantity <= 0)
        {
            RemoveItem(productId);
            return;
        }

        item.UpdateQuantity(quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(int productId)
    {
        var item = Items.FirstOrDefault(
            x => x.ProductId == productId);

        if (item is null)
            throw new InvalidOperationException("Product is not in the cart.");

        Items.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Clear()
    {
        Items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }
}
