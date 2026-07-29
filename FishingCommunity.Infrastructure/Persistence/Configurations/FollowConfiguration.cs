using FishingCommunity.Domain.Entities.Community;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.HasIndex(f => new { f.FollowerId, f.FollowedId }).IsUnique();
        builder.HasIndex(f => f.FollowedId);
        builder.HasIndex(f => f.FollowerId);
    }
}