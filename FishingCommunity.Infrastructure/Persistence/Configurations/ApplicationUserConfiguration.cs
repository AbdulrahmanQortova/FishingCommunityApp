using FishingCommunity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(2048);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(u => u.CreatedDate)
            .IsRequired();

        // Global query filter: automatically excludes soft-deleted users from every query
        // unless explicitly overridden with IgnoreQueryFilters().
        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Status);
    }
}