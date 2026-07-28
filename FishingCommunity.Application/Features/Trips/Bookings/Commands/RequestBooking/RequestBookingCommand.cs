using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.RequestBooking;

public class RequestBookingCommand : IRequest<Result<RequestBookingResponse>>
{
    public Guid TripId { get; set; }
    public Guid UserId { get; set; }
    public int SeatsRequested { get; set; } = 1;
}