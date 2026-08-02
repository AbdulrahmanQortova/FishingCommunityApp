using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Trips.Commands.AdminCancelTrip;

public class AdminCancelTripCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public string Reason { get; set; } = string.Empty;
}