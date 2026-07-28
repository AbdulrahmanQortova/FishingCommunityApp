using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.RequestBooking;

public class RequestBookingResponse
{
    public Guid BookingId { get; set; }
    public Guid TripId { get; set; }
    public int SeatsRequested { get; set; }
    public BookingStatus Status { get; set; }
    public bool WasAddedToWaitingList { get; set; }
}