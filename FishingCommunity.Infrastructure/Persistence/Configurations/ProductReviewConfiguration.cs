using FishingCommunity.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.HasIndex(r => new { r.ProductId, r.UserId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}