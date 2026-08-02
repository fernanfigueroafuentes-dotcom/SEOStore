using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Commerce;

namespace SEOStore.Infrastructure.Persistence.Configurations.Commerce;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.UserId)
            .HasMaxLength(450);


        builder.Property(x => x.CreatedAt)
            .IsRequired();


        builder.Property(x => x.UpdatedAt);


        builder.HasMany(x => x.Items)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}