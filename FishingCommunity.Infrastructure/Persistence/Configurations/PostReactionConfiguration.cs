using FishingCommunity.Domain.Entities.Community;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.Property(r => r.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // One reaction per user per post — enforced at the DB level too.
        // No soft-delete filter needed: PostReaction inherits BaseEntity, not
        // BaseAuditableEntity, so there's no IsDeleted column to filter on.
        builder.HasIndex(r => new { r.PostId, r.UserId }).IsUnique();
    }
}