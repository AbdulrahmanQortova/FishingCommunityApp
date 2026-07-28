using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Commands.UpdateTrip;

public class UpdateTripCommandHandler : IRequestHandler<UpdateTripCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTripCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _unitOfWork.Repository<Trip>().GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
        {
            throw new NotFoundException(nameof(Trip), request.TripId);
        }

        if (trip.OrganizerId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to update this trip.");
        }

        // If this violates a business rule (e.g. trip already started/completed),
        // UpdateDetails() throws BusinessRuleValidationException, which propagates up
        // and is translated into a 400 response by GlobalExceptionHandlerMiddleware —
        // no try/catch needed here.
        trip.UpdateDetails(request.Title, request.Description, request.DepartureDateTime, request.EstimatedReturnDateTime, request.PricePerPerson);

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Trip updated successfully.");
    }
}