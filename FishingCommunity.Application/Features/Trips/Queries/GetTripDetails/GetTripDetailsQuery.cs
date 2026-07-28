using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Queries.GetTripDetails;

public class GetTripDetailsQuery : IRequest<Result<TripDetailsDto>>
{
    public Guid TripId { get; set; }
}