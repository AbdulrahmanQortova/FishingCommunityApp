using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.ApproveBooking;

public class ApproveBookingCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid BookingId { get; set; }
    public Guid RequestingUserId { get; set; } // Must be the trip organizer
}