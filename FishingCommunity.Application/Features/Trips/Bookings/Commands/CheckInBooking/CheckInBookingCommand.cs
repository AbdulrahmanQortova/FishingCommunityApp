using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.CheckInBooking;

public class CheckInBookingCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid BookingId { get; set; }
    public Guid RequestingUserId { get; set; } // Must be the trip organizer
}