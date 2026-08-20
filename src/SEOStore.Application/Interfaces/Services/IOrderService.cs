using SEOStore.Application.Features.Orders.DTOs;

namespace SEOStore.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderDto> CheckoutAsync(string userId, CheckoutDto dto, CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderDto>> GetMineAsync(string userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<OrderDto?> GetByIdAsync(int id, string userId, bool isAdmin, CancellationToken cancellationToken = default);

    Task<OrderDto> ChangeStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);

    Task<PaymentDto> AddPaymentAsync(int orderId, string userId, bool isAdmin, CreatePaymentDto dto, CancellationToken cancellationToken = default);

    Task<PaymentDto> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusDto dto, CancellationToken cancellationToken = default);
}
