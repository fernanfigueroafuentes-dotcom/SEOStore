using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Catalog;

namespace SEOStore.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration 
    : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.HasKey(x => x.Id);


        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(500);


        builder.Property(x => x.DisplayOrder)
            .HasDefaultValue(0);


        builder.Property(x => x.IsPrimary)
            .HasDefaultValue(false);



        builder.HasOne(x => x.Product)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}