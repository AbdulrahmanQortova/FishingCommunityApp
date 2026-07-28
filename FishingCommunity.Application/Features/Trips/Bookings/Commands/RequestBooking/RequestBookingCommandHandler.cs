using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.RequestBooking;

public class RequestBookingCommandHandler : IRequestHandler<RequestBookingCommand, Result<RequestBookingResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RequestBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RequestBookingResponse>> Handle(RequestBookingCommand request, CancellationToken cancellationToken)
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

        // If the trip is full, join the waiting list instead of failing outright —
        // a smoother UX than forcing the user to manually retry via a separate endpoint.
        if (trip.IsFull)
        {
            var waitingEntry = trip.JoinWaitingList(request.UserId, request.SeatsRequested);

            _unitOfWork.Repository<Trip>().Update(trip);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<RequestBookingResponse>.Success(new RequestBookingResponse
            {
                BookingId = waitingEntry.Id,
                TripId = trip.Id,
                SeatsRequested = request.SeatsRequested,
                WasAddedToWaitingList = true
            }, "Trip is currently full — you've been added to the waiting list.");
        }

        var booking = trip.RequestBooking(request.UserId, request.SeatsRequested);

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RequestBookingResponse
        {
            BookingId = booking.Id,
            TripId = trip.Id,
            SeatsRequested = booking.SeatsRequested,
            Status = booking.Status,
            WasAddedToWaitingList = false
        };

        return Result<RequestBookingResponse>.Success(response, "Booking request submitted successfully.");
    }
}