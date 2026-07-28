using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.RejectBooking;

public class RejectBookingCommand : IRequest<Result>
{
    public Guid TripId { get; set; }
    public Guid BookingId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string? Reason { get; set; }
}