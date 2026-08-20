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

    public string? ShippingStreet { get; set; }

    public string? ShippingCity { get; set; }

    public string? ShippingRegion { get; set; }

    public string? ShippingPostalCode { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}