using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.FishingRecords;

public class FishSpecies : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? ScientificName { get; private set; }
    public string? IconUrl { get; private set; }

    private FishSpecies() { } // EF Core

    public FishSpecies(string name, string? scientificName = null, string? iconUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Fish species name is required.");
        }

        Name = name;
        ScientificName = scientificName;
        IconUrl = iconUrl;
    }
}