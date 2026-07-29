using FishingCommunity.Domain.Entities.Community;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class SavedPostConfiguration : IEntityTypeConfiguration<SavedPost>
{
    public void Configure(EntityTypeBuilder<SavedPost> builder)
    {
        builder.HasOne(s => s.Post)
            .WithMany()
            .HasForeignKey(s => s.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.PostId, s.UserId }).IsUnique();
        builder.HasIndex(s => s.UserId);
    }
}