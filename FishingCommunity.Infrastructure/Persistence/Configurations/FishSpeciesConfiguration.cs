using FishingCommunity.Domain.Entities.FishingRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FishingCommunity.Infrastructure.Persistence.Configurations;

public class FishSpeciesConfiguration : IEntityTypeConfiguration<FishSpecies>
{
    public void Configure(EntityTypeBuilder<FishSpecies> builder)
    {
        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.ScientificName)
            .HasMaxLength(150);

        builder.Property(f => f.IconUrl)
            .HasMaxLength(2048);

        builder.HasQueryFilter(f => !f.IsDeleted);

        builder.HasIndex(f => f.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}