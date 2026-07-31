using FishingCommunity.Domain.Entities.Map;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class FavoriteLocationConfiguration : IEntityTypeConfiguration<FavoriteLocation>
{
    public void Configure(EntityTypeBuilder<FavoriteLocation> builder)
    {
        builder.HasOne(f => f.FishingSpot)
            .WithMany()
            .HasForeignKey(f => f.FishingSpotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.UserId, f.FishingSpotId }).IsUnique();
    }
}