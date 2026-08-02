using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Content;

namespace SEOStore.Infrastructure.Persistence.Configurations.Content;

public class BlogPostConfiguration 
    : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable("BlogPosts");

        builder.HasKey(x => x.Id);



        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);



        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);


        builder.HasIndex(x => x.Slug)
            .IsUnique();



        builder.Property(x => x.Summary)
            .IsRequired()
            .HasMaxLength(500);



        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("text");



        builder.Property(x => x.FeaturedImageUrl)
            .HasMaxLength(500);



        builder.Property(x => x.Author)
            .HasMaxLength(150);



        builder.Property(x => x.Published)
            .HasDefaultValue(false);



        builder.Property(x => x.ViewCount)
            .HasDefaultValue(0);



        builder.HasIndex(x => new
        {
            x.Published,
            x.PublishedAt
        });


        builder.HasIndex(x => x.DisplayOrder);
    }
}