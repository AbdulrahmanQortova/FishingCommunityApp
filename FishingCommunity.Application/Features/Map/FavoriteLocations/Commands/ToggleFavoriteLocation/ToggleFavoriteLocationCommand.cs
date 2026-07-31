using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FavoriteLocations.Commands.ToggleFavoriteLocation;

public class ToggleFavoriteLocationCommand : IRequest<Result<bool>> // true = added, false = removed
{
    public Guid UserId { get; set; }
    public Guid FishingSpotId { get; set; }
}