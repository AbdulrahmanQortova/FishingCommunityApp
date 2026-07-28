using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Bookings.Queries.GetMyBookings;

public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, Result<List<MyBookingDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyBookingsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<MyBookingDto>>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _unitOfWork.Repository<TripBooking>().Query()
            .Where(b => b.UserId == request.UserId)
            .OrderByDescending(b => b.CreatedDate)
            .Select(b => new MyBookingDto
            {
                BookingId = b.Id,
                TripId = b.TripId,
                TripTitle = b.Trip.Title,
                DepartureDateTime = b.Trip.DepartureDateTime,
                SeatsRequested = b.SeatsRequested,
                Status = b.Status
            })
            .ToListAsync(cancellationToken);

        return Result<List<MyBookingDto>>.Success(bookings);
    }
}