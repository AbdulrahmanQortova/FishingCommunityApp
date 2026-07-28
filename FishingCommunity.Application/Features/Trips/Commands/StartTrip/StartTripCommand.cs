using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Commands.StartTrip;

public class StartTripCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid RequestingUserId { get; set; }
}