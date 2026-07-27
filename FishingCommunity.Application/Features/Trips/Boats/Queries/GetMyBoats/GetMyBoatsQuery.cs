using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Boats.Queries.GetMyBoats;

public class GetMyBoatsQuery : IRequest<Result<List<BoatDto>>>
{
    public Guid OwnerId { get; set; }
}