using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Infrastructure.Persistence.Configurations.Commerce;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasIndex(x => new { x.CartId, x.ProductId })
            .IsUnique();


        builder.Property(x => x.UnitPrice)
            .HasPrecision(10, 2)
            .IsRequired();


        builder.HasOne(x => x.Product)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
