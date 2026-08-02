using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Commerce;
using SEOStore.Domain.Entities.Commerce.Enums;

namespace SEOStore.Infrastructure.Persistence.Configurations.Commerce;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);


        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();


        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();


        builder.Property(x => x.SubTotal)
            .HasPrecision(10, 2);


        builder.Property(x => x.ShippingCost)
            .HasPrecision(10, 2);


        builder.Property(x => x.Discount)
            .HasPrecision(10, 2);


        builder.Property(x => x.Total)
            .HasPrecision(10, 2);


        builder.Property(x => x.Notes)
            .HasColumnType("text");


        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}