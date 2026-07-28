using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.CancelBooking;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var trip = await _unitOfWork.Repository<Trip>().Query()
            .Where(t => t.Id == request.TripId)
            .Include(t => t.Bookings)
            .Include(t => t.WaitingList)
            .FirstOrDefaultAsync(cancellationToken);

        if (trip is null)
        {
            throw new NotFoundException(nameof(Trip), request.TripId);
        }

        var booking = trip.Bookings.FirstOrDefault(b => b.Id == request.BookingId);

        if (booking is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Trips.TripBooking), request.BookingId);
        }

        if (booking.UserId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to cancel this booking.");
        }

        trip.CancelBooking(request.BookingId, request.RequestingUserId);

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Booking cancelled successfully.");
    }
}