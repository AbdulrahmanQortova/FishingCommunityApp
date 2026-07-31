using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Map.FavoriteLocations.Queries.GetMyFavoriteLocations;

public class GetMyFavoriteLocationsQuery : IRequest<Result<List<FavoriteLocationDto>>>
{
    public Guid UserId { get; set; }
}