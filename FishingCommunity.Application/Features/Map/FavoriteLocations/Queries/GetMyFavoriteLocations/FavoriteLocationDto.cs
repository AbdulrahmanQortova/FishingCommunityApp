using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Map.FavoriteLocations.Queries.GetMyFavoriteLocations;

public class FavoriteLocationDto
{
    public Guid FishingSpotId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public FishingSpotType Type { get; set; }
}