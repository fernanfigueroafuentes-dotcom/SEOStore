using SEOStore.Domain.Entities.Commerce.Enums;

namespace SEOStore.Application.Features.Orders.DTOs;

public class CheckoutDto
{
    public int? AddressId { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal Discount { get; set; }

    public string? Notes { get; set; }

    public string? PaymentMethod { get; set; }
}

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}

public class CreatePaymentDto
{
    public string PaymentMethod { get; set; } = "Coordinated";

    public decimal? Amount { get; set; }

    public string? TransactionId { get; set; }
}

public class UpdatePaymentStatusDto
{
    public PaymentStatus Status { get; set; }

    public string? TransactionId { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

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

    public DateTime CreatedAt { get; set; }

    public List<OrderItemDto> Items { get; set; } = [];

    public List<PaymentDto> Payments { get; set; } = [];
}

public class OrderItemDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Total { get; set; }
}

public class PaymentDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }
}
