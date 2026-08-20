using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Seo;

namespace SEOStore.Infrastructure.Persistence.Configurations.Seo;

public class SlugRedirectConfiguration : IEntityTypeConfiguration<SlugRedirect>
{
    public void Configure(EntityTypeBuilder<SlugRedirect> builder)
    {
        builder.ToTable("SlugRedirects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OldPath)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.NewPath)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasIndex(x => x.OldPath)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = FALSE");
    }
}
