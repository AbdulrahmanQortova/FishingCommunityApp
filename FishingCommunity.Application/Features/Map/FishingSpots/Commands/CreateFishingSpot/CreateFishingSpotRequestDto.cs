using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Commands.CreateFishingSpot;

public class CreateFishingSpotRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public FishingSpotType Type { get; set; }
}