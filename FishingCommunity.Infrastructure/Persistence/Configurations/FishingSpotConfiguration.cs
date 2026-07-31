using FishingCommunity.Domain.Entities.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class FishingSpotConfiguration : IEntityTypeConfiguration<FishingSpot>
{
    public void Configure(EntityTypeBuilder<FishingSpot> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.Property(s => s.Latitude).HasColumnType("float");
        builder.Property(s => s.Longitude).HasColumnType("float");

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
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

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasIndex(s => new { s.Latitude, s.Longitude });
        builder.HasIndex(s => s.IsVerified);
    }
}