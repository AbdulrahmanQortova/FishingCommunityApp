using FishingCommunity.Domain.Entities.Shop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(3000);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.MainPhotoUrl)
            .HasMaxLength(2048);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property<List<string>>("_photoUrls")
            .HasField("_photoUrls")
            .HasColumnName("PhotoUrls")
            .HasConversion(
                v => string.Join(';', v),
                v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.Name);
    }
}