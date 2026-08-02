using SEOStore.Domain.Common;

namespace SEOStore.Infrastructure.Identity;

public class Address : BaseEntity
{
    public string UserId { get; set; } = null!;


    public ApplicationUser User { get; set; } = null!;


    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}