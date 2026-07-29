using FishingCommunity.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.Description)
            .HasMaxLength(2000);

        builder.Property(s => s.LogoUrl).HasMaxLength(2048);
        builder.Property(s => s.BannerUrl).HasMaxLength(2048);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasMany(s => s.Products)
            .WithOne(p => p.Store)
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasIndex(s => s.OwnerId).IsUnique(); // One store per owner (per current business rule).
    }
}