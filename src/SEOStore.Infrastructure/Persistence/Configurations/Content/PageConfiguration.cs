using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Content;

namespace SEOStore.Infrastructure.Persistence.Configurations.Content;

public class PageConfiguration 
    : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.ToTable("Pages");

        builder.HasKey(x => x.Id);



        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);



        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);


        builder.HasIndex(x => x.Slug)
            .IsUnique();



        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("text");



        builder.Property(x => x.Published)
            .HasDefaultValue(false);



        builder.HasIndex(x => x.Published);
    }
}