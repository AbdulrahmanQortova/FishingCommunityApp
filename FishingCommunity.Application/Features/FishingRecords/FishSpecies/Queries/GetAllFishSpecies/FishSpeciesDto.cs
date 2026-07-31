namespace FishingCommunity.Application.Features.FishingRecords.FishSpecies.Queries.GetAllFishSpecies;

public class FishSpeciesDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ScientificName { get; set; }
    public string? IconUrl { get; set; }
}