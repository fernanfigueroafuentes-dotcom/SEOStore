using SEOStore.Domain.Common;
using SEOStore.Domain.Entities.Commerce.Enums;

namespace SEOStore.Domain.Entities.Commerce;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public string PaymentMethod { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? TransactionId { get; set; }

    public DateTime? PaidAt { get; set; }
}
