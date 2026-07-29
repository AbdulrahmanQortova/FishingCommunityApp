using FishingCommunity.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.CouponCode).HasMaxLength(50);
        builder.Property(o => o.DiscountAmount).HasColumnType("decimal(10,2)");
        builder.Property(o => o.SubtotalAmount).HasColumnType("decimal(10,2)");
        builder.Property(o => o.TotalAmount).HasColumnType("decimal(10,2)");
        builder.Property(o => o.CancellationReason).HasMaxLength(500);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(o => o.ShippingAddress)
            .WithMany()
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(o => !o.IsDeleted);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
    }
}