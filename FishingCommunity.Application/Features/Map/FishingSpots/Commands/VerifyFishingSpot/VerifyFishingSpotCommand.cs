using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Commands.VerifyFishingSpot;

public class VerifyFishingSpotCommand : IRequest<Result>
{
    public Guid FishingSpotId { get; set; }
}