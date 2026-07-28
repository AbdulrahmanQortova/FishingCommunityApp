using FishingCommunity.Domain.Entities.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class TripReviewConfiguration : IEntityTypeConfiguration<TripReview>
{
    public void Configure(EntityTypeBuilder<TripReview> builder)
    {
        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasIndex(r => new { r.TripId, r.UserId })
     .IsUnique()
     .HasFilter("[IsDeleted] = 0");
    }
}