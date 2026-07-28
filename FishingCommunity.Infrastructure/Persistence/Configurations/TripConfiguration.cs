using FishingCommunity.Domain.Entities.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.LocationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Latitude).HasColumnType("float");
        builder.Property(t => t.Longitude).HasColumnType("float");

        builder.Property(t => t.PricePerPerson)
            .HasColumnType("decimal(10,2)");

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property<List<string>>("_photoUrls")
            .HasField("_photoUrls")
            .HasColumnName("PhotoUrls")
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        builder.HasOne(t => t.Boat)
            .WithMany(b => b.Trips)
            .HasForeignKey(t => t.BoatId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a boat that has trips.

        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Trip)
            .HasForeignKey(b => b.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.WaitingList)
            .WithOne(w => w.Trip)
            .HasForeignKey(w => w.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Reviews)
            .WithOne(r => r.Trip)
            .HasForeignKey(r => r.TripId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasIndex(t => t.DepartureDateTime);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.OrganizerId);
        builder.HasIndex(t => new { t.Latitude, t.Longitude });
    }
}