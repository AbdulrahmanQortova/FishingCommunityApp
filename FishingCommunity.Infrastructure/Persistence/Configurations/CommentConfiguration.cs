using FishingCommunity.Domain.Entities.Community;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(1000);

        // Self-referencing relationship for reply threading (ParentCommentId).
        // Restrict prevents cascade-delete cycles, which SQL Server disallows anyway
        // for self-referencing FKs; the interceptor's soft-delete keeps rows regardless.
        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.PostId);
        builder.HasIndex(c => c.UserId);
    }
}