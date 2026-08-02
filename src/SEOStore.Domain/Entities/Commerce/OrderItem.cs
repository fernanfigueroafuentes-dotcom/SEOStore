using SEOStore.Domain.Entities.Catalog;
using SEOStore.Domain.Common;

namespace SEOStore.Domain.Entities.Commerce;

public class OrderItem : BaseEntity
{

    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;



    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;



    public string ProductName { get; set; } = null!;


    public decimal UnitPrice { get; set; }


    public int Quantity { get; set; }


    public decimal Total { get; set; }
}