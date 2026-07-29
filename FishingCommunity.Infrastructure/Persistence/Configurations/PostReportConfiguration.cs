using FishingCommunity.Domain.Entities.Community;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class PostReportConfiguration : IEntityTypeConfiguration<PostReport>
{
    public void Configure(EntityTypeBuilder<PostReport> builder)
    {
        builder.Property(r => r.Reason)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.AdditionalDetails)
            .HasMaxLength(1000);

        builder.Property(r => r.ResolutionNotes)
            .HasMaxLength(1000);

        // No navigation to Post configured deliberately (PostReport isn't part of
        // the Post aggregate — see the Domain layer notes). Just a plain FK-like Guid,
        // so no HasOne/WithMany relationship is declared here.

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasIndex(r => r.PostId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.PostId, r.ReportedByUserId }).IsUnique();
    }
}