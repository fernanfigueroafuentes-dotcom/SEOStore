using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Infrastructure.Persistence.Configurations.Commerce;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(200);


        builder.Property(x => x.UnitPrice)
            .HasPrecision(10, 2)
            .IsRequired();


        builder.Property(x => x.Total)
            .HasPrecision(10, 2)
            .IsRequired();


        builder.Property(x => x.Quantity)
            .IsRequired();


        builder.HasOne(x => x.Product)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}