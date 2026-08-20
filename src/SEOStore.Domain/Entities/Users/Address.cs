using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Users;

public class Address : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
