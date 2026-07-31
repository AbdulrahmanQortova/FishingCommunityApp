using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Queries.GetNearbyFishingSpots;

public class FishingSpotDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public FishingSpotType Type { get; set; }
    public bool IsVerified { get; set; }
    public double DistanceKm { get; set; }
}