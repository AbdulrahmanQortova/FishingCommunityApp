using FishingCommunity.Domain.Enums;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Commands.CreateFishingSpot;

public class CreateFishingSpotCommand : IRequest<Result<Guid>>
{
    public Guid CreatedByUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public FishingSpotType Type { get; set; }
}