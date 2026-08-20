using SEOStore.Application.Features.Orders.DTOs;
using SEOStore.Application.Interfaces.Repositories;
using SEOStore.Application.Interfaces.Services;
using SEOStore.Domain.Entities.Commerce;
using SEOStore.Domain.Entities.Commerce.Enums;

namespace SEOStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ISettingService _settingService;

    public OrderService(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IAddressRepository addressRepository,
        ISettingService settingService)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _addressRepository = addressRepository;
        _settingService = settingService;
    }

    public async Task<OrderDto> CheckoutAsync(string userId, CheckoutDto dto, CancellationToken cancellationToken = default)
    {
        var site = await _settingService.GetCurrentAsync(cancellationToken);
        if (!site.CheckoutEnabled)
            throw new InvalidOperationException("Checkout is disabled in catalog mode.");

        if (dto.ShippingCost < 0 || dto.Discount < 0)
            throw new ArgumentException("Shipping and discount cannot be negative.");

        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
            throw new InvalidOperationException("The cart is empty.");

        foreach (var item in cart.Items)
        {
            if (item.Product is null || item.Product.IsDeleted || !item.Product.Published)
                throw new InvalidOperationException("A product in the cart is no longer available.");

            item.Product.DecrementStock(item.Quantity);
        }

        var subTotal = cart.Items.Sum(item => item.GetTotal());
        if (dto.Discount > subTotal)
            throw new ArgumentException("Discount cannot exceed the subtotal.");

        var order = new Order
        {
            UserId = userId,
            OrderNumber = NewOrderNumber(),
            Status = OrderStatus.Pending,
            SubTotal = subTotal,
            ShippingCost = dto.ShippingCost,
            Discount = dto.Discount,
            Total = subTotal + dto.ShippingCost - dto.Discount,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        if (dto.AddressId is > 0)
        {
            var address = await _addressRepository.GetByIdAsync(dto.AddressId.Value, userId, cancellationToken)
                ?? throw new KeyNotFoundException("The shipping address was not found.");
            order.ShippingStreet = address.Street;
            order.ShippingCity = address.City;
            order.ShippingRegion = address.Region;
            order.ShippingPostalCode = address.PostalCode;
        }

        foreach (var item in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                Total = item.GetTotal(),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
        {
            order.Payments.Add(new Payment
            {
                PaymentMethod = dto.PaymentMethod.Trim(),
                Amount = order.Total,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            });
        }

        cart.Clear();
        await _orderRepository.AddAsync(order, cancellationToken);
        return Map(order);
    }

    public async Task<IEnumerable<OrderDto>> GetMineAsync(string userId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);
        return orders.Select(Map);
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(Map);
    }

    public async Task<OrderDto?> GetByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return null;
        if (!isAdmin && !string.Equals(order.UserId, userId, StringComparison.Ordinal))
            return null;
        return Map(order);
    }

    public async Task<OrderDto> ChangeStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("The order was not found.");

        if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
            throw new InvalidOperationException("A completed or cancelled order cannot change status.");

        var previous = order.Status;
        order.Status = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == OrderStatus.Cancelled && previous != OrderStatus.Cancelled)
            await RestoreStockAsync(order, cancellationToken);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        return Map(order);
    }

    public async Task<PaymentDto> AddPaymentAsync(
        int orderId,
        string userId,
        bool isAdmin,
        CreatePaymentDto dto,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("The order was not found.");

        if (!isAdmin && !string.Equals(order.UserId, userId, StringComparison.Ordinal))
            throw new KeyNotFoundException("The order was not found.");

        if (order.Status is OrderStatus.Cancelled or OrderStatus.Completed)
            throw new InvalidOperationException("Payments cannot be added to this order.");

        var method = string.IsNullOrWhiteSpace(dto.PaymentMethod) ? "Coordinated" : dto.PaymentMethod.Trim();
        var paid = order.Payments.Where(payment => payment.Status == PaymentStatus.Completed).Sum(payment => payment.Amount);
        var remaining = order.Total - paid;
        var amount = dto.Amount ?? remaining;
        if (amount <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.");
        if (amount > remaining)
            throw new ArgumentException("Payment amount exceeds the remaining balance.");

        var payment = new Payment
        {
            OrderId = order.Id,
            PaymentMethod = method,
            Amount = amount,
            Status = PaymentStatus.Pending,
            TransactionId = string.IsNullOrWhiteSpace(dto.TransactionId) ? null : dto.TransactionId.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        await _paymentRepository.AddAsync(payment, cancellationToken);
        return MapPayment(payment);
    }

    public async Task<PaymentDto> UpdatePaymentStatusAsync(
        int paymentId,
        UpdatePaymentStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken)
            ?? throw new KeyNotFoundException("The payment was not found.");

        payment.Status = dto.Status;
        payment.UpdatedAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(dto.TransactionId))
            payment.TransactionId = dto.TransactionId.Trim();

        if (dto.Status == PaymentStatus.Completed)
        {
            payment.PaidAt ??= DateTime.UtcNow;
            if (payment.Order.Status == OrderStatus.Pending)
            {
                payment.Order.Status = OrderStatus.Confirmed;
                payment.Order.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _paymentRepository.UpdateAsync(payment, cancellationToken);
        return MapPayment(payment);
    }

    private async Task RestoreStockAsync(Order order, CancellationToken cancellationToken)
    {
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                continue;

            product.RestoreStock(item.Quantity);
        }
    }

    private static string NewOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

    private static OrderDto Map(Order order) => new()
    {
        Id = order.Id,
        UserId = order.UserId,
        OrderNumber = order.OrderNumber,
        Status = order.Status,
        SubTotal = order.SubTotal,
        ShippingCost = order.ShippingCost,
        Discount = order.Discount,
        Total = order.Total,
        Notes = order.Notes,
        ShippingStreet = order.ShippingStreet,
        ShippingCity = order.ShippingCity,
        ShippingRegion = order.ShippingRegion,
        ShippingPostalCode = order.ShippingPostalCode,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(item => new OrderItemDto
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            Total = item.Total
        }).ToList(),
        Payments = order.Payments.Select(MapPayment).ToList()
    };

    private static PaymentDto MapPayment(Payment payment) => new()
    {
        Id = payment.Id,
        OrderId = payment.OrderId,
        PaymentMethod = payment.PaymentMethod,
        Amount = payment.Amount,
        Status = payment.Status,
        TransactionId = payment.TransactionId,
        PaidAt = payment.PaidAt
    };
}
