using FishingCommunity.Domain.Enums;

namespace FishingCommunity.Application.Features.Trips.Bookings.Queries.GetMyBookings;

public class MyBookingDto
{
    public Guid BookingId { get; set; }
    public Guid TripId { get; set; }
    public string TripTitle { get; set; } = string.Empty;
    public DateTime DepartureDateTime { get; set; }
    public int SeatsRequested { get; set; }
    public BookingStatus Status { get; set; }
}