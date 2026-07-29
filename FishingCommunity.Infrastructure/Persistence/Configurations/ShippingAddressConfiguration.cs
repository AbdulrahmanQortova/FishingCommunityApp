using FishingCommunity.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.Property(a => a.FullName).IsRequired().HasMaxLength(150);
        builder.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(a => a.AddressLine1).IsRequired().HasMaxLength(300);
        builder.Property(a => a.AddressLine2).HasMaxLength(300);
        builder.Property(a => a.City).IsRequired().HasMaxLength(100);
        builder.Property(a => a.State).HasMaxLength(100);
        builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
        builder.Property(a => a.PostalCode).HasMaxLength(20);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.HasIndex(a => a.UserId);
    }
}