using FishingCommunity.Domain.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.Property(c => c.LastMessagePreview)
            .HasMaxLength(150);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(c => !c.IsDeleted);

        // Ensures the normalized (ParticipantOneId, ParticipantTwoId) pair is unique —
        // combined with the Domain layer's ordering logic, this guarantees exactly one
        // conversation exists between any two users.
        builder.HasIndex(c => new { c.ParticipantOneId, c.ParticipantTwoId }).IsUnique();
    }
}