using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Commerce;

public class Cart : BaseEntity
{
    public string? UserId { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public DateTime? UpdatedAt { get; set; }


    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}