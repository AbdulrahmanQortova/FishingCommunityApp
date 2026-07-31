using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Queries.GetNearbyFishingSpots;

public class GetNearbyFishingSpotsQuery : IRequest<Result<List<FishingSpotDto>>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 50; // Default search radius
}