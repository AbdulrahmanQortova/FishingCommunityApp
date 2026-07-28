using FishingCommunity.Domain.Entities.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class TripWaitingListEntryConfiguration : IEntityTypeConfiguration<TripWaitingListEntry>
{
    public void Configure(EntityTypeBuilder<TripWaitingListEntry> builder)
    {
        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.HasIndex(w => new { w.TripId, w.UserId });
    }
}