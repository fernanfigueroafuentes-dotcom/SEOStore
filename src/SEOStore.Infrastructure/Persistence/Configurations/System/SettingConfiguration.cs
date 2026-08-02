using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SEOStore.Domain.Entities.Configuration;

namespace SEOStore.Infrastructure.Persistence.Configurations.System;

public class SettingConfiguration 
    : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings");

        builder.HasKey(x => x.Id);



        builder.Property(x => x.SiteName)
            .IsRequired()
            .HasMaxLength(150);



        builder.Property(x => x.LogoUrl)
            .HasMaxLength(500);



        builder.Property(x => x.FaviconUrl)
            .HasMaxLength(500);



        builder.Property(x => x.Phone)
            .HasMaxLength(50);



        builder.Property(x => x.Email)
            .HasMaxLength(150);



        builder.Property(x => x.WhatsApp)
            .HasMaxLength(50);



        builder.Property(x => x.Facebook)
            .HasMaxLength(300);



        builder.Property(x => x.Instagram)
            .HasMaxLength(300);



        builder.Property(x => x.Address)
            .HasMaxLength(300);



        builder.Property(x => x.PrimaryColor)
            .HasMaxLength(20)
            .HasDefaultValue("#000000");



        builder.Property(x => x.SecondaryColor)
            .HasMaxLength(20)
            .HasDefaultValue("#FFFFFF");



        builder.Property(x => x.GoogleAnalytics)
            .HasMaxLength(100);



        builder.Property(x => x.GoogleTagManager)
            .HasMaxLength(100);
    }
}