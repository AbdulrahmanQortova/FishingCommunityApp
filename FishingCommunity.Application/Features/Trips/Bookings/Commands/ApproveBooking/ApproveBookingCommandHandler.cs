using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Bookings.Commands.ApproveBooking;

public class ApproveBookingCommandHandler : IRequestHandler<ApproveBookingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ApproveBookingCommand request, CancellationToken cancellationToken)
    {
        var trip = await _unitOfWork.Repository<Trip>().Query()
            .Where(t => t.Id == request.TripId)
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(cancellationToken);

        if (trip is null)
        {
            throw new NotFoundException(nameof(Trip), request.TripId);
        }

        if (trip.OrganizerId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to manage bookings for this trip.");
        }

        trip.ApproveBooking(request.BookingId);

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Booking approved successfully.");
    }
}