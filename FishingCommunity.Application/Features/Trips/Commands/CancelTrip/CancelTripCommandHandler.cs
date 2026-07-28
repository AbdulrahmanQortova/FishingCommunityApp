using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Commands.CancelTrip;

public class CancelTripCommandHandler : IRequestHandler<CancelTripCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelTripCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _unitOfWork.Repository<Trip>().GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
        {
            throw new NotFoundException(nameof(Trip), request.TripId);
        }

        if (trip.OrganizerId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to cancel this trip.");
        }

        // Trip.Cancel() throws BusinessRuleValidationException if the trip already
        // started/completed/was cancelled — propagates naturally to the middleware.
        trip.Cancel(request.Reason);

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Trip cancelled successfully.");
    }
}