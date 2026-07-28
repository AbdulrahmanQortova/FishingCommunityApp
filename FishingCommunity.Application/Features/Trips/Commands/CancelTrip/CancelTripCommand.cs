using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Commands.CancelTrip;

public class CancelTripCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string? Reason { get; set; }
}