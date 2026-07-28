using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Trips.Commands.CompleteTrip;

public class CompleteTripCommandHandler : IRequestHandler<CompleteTripCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompleteTripCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CompleteTripCommand request, CancellationToken cancellationToken)
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
            return Result.Failure("You are not authorized to complete this trip.");
        }

        // Trip.Complete() marks all CheckedIn bookings as Completed —
        // needs Bookings loaded via Include, same reasoning as RequestBooking.
        trip.Complete();

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Trip completed.");
    }
}