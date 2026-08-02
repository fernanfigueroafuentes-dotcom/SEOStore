using SEOStore.Domain.Common;
using SEOStore.Domain.Entities.Commerce.Enums;

namespace SEOStore.Domain.Entities.Commerce;

public class Order : BaseEntity
{

    public string? UserId { get; set; }


    public string OrderNumber { get; set; } = null!;


    public OrderStatus Status { get; set; }


    public decimal SubTotal { get; set; }


    public decimal ShippingCost { get; set; }


    public decimal Discount { get; set; }


    public decimal Total { get; set; }


    public string? Notes { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    public ICollection<OrderItem> Items { get; set; }
        = new List<OrderItem>();
}