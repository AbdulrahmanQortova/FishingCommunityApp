using FishingCommunity.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.Amount)
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Method)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.SenderPhoneOrHandle).HasMaxLength(50);
        builder.Property(p => p.TransferProofUrl).HasMaxLength(2048);
        builder.Property(p => p.RejectionReason).HasMaxLength(500);

        // Deliberately no FK relationship to Order — Payment references OrderId as a
        // plain Guid, consistent with keeping Payment as its own independent Aggregate
        // Root (same reasoning as PostReport not being tied to Post via FK).

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.Status);
    }
}