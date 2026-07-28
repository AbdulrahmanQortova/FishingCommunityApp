using FishingCommunity.Domain.Entities.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class TripBookingConfiguration : IEntityTypeConfiguration<TripBooking>
{
    public void Configure(EntityTypeBuilder<TripBooking> builder)
    {
        builder.Property(b => b.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(b => b.RejectionReason)
            .HasMaxLength(500);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasIndex(b => new { b.TripId, b.UserId });
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.Status);
    }
}