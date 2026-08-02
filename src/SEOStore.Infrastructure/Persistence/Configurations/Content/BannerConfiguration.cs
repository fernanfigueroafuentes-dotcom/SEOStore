using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Content;

namespace SEOStore.Infrastructure.Persistence.Configurations.Content;

public class BannerConfiguration 
    : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("Banners");

        builder.HasKey(x => x.Id);



        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);



        builder.Property(x => x.Subtitle)
            .HasMaxLength(300);



        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);



        builder.Property(x => x.Link)
            .HasMaxLength(500);



        builder.Property(x => x.DisplayOrder)
            .HasDefaultValue(0);



        builder.Property(x => x.Active)
            .HasDefaultValue(true);



        builder.HasIndex(x => new
        {
            x.Active,
            x.DisplayOrder
        });
    }
}