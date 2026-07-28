using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Bookings.Queries.GetMyBookings;

public class GetMyBookingsQuery : IRequest<Result<List<MyBookingDto>>>
{
    public Guid UserId { get; set; }
}