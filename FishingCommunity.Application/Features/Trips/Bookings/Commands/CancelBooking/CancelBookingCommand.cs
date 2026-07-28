using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.CancelBooking;

public class CancelBookingCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid BookingId { get; set; }
    public Guid RequestingUserId { get; set; } // Must be the booking owner (the user who booked)
}