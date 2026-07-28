using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Commands.CompleteTrip;

public class CompleteTripCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid RequestingUserId { get; set; }
}