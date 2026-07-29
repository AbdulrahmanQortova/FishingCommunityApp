using FishingCommunity.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Value).HasColumnType("decimal(10,2)");
        builder.Property(c => c.MinOrderAmount).HasColumnType("decimal(10,2)");

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}