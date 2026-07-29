using FishingCommunity.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(200);
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(10,2)");

        // Deliberately no FK relationship to Product — OrderItem stores an immutable
        // snapshot (ProductName, UnitPrice) and only keeps ProductId as a soft reference
        // for potential future lookups, consistent with the Domain layer notes.
        builder.HasIndex(i => i.ProductId);
    }
}