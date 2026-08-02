using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Integrations;

namespace SEOStore.Infrastructure.Persistence.Configurations.Integrations;

public class MercadoLibreAuthConfiguration 
    : IEntityTypeConfiguration<MercadoLibreAuth>
{
    public void Configure(EntityTypeBuilder<MercadoLibreAuth> builder)
    {
        builder.ToTable("MercadoLibreAuths");


        builder.HasKey(x => x.Id);



        builder.Property(x => x.UserId)
            .IsRequired();



        builder.Property(x => x.AccessToken)
            .IsRequired()
            .HasMaxLength(500);



        builder.Property(x => x.RefreshToken)
            .HasMaxLength(500);



        builder.Property(x => x.TokenType)
            .HasMaxLength(50);



        builder.HasIndex(x => x.UserId)
            .IsUnique();
    }
}