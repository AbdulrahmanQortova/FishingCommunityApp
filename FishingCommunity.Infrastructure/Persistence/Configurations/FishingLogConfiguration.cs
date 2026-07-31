using FishingCommunity.Domain.Entities.FishingRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class FishingLogConfiguration : IEntityTypeConfiguration<FishingLog>
{
    public void Configure(EntityTypeBuilder<FishingLog> builder)
    {
        builder.Property(l => l.LocationName).HasMaxLength(200);
        builder.Property(l => l.Latitude).HasColumnType("float");
        builder.Property(l => l.Longitude).HasColumnType("float");
        builder.Property(l => l.Bait).HasMaxLength(200);
        builder.Property(l => l.Notes).HasMaxLength(2000);
        builder.Property(l => l.WeatherDescription).HasMaxLength(200);

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

        builder.HasOne(l => l.FishSpecies)
            .WithMany()
            .HasForeignKey(l => l.FishSpeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.CaughtDate);
        builder.HasIndex(l => new { l.Latitude, l.Longitude });
    }
}