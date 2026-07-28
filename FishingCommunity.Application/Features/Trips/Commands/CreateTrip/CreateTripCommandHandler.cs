using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Commands.CreateTrip;

public class CreateTripCommandHandler : IRequestHandler<CreateTripCommand, Result<CreateTripResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTripCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateTripResponse>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        var boat = await _unitOfWork.Repository<Boat>().GetByIdAsync(request.BoatId, cancellationToken);

        if (boat is null)
        {
            throw new NotFoundException(nameof(Boat), request.BoatId);
        }

        if (boat.OwnerId != request.OrganizerId)
        {
            return Result<CreateTripResponse>.Failure("You can only create trips for boats you own.");
        }

        if (boat.Status != BoatStatus.Active)
        {
            return Result<CreateTripResponse>.Failure("This boat is not currently active and cannot be scheduled for trips.");
        }

        if (request.Capacity > boat.Capacity)
        {
            return Result<CreateTripResponse>.Failure($"Trip capacity cannot exceed the boat's capacity ({boat.Capacity}).");
        }

        var trip = new Trip(
            request.BoatId,
            request.OrganizerId,
            request.Title,
            request.LocationName,
            request.Latitude,
            request.Longitude,
            request.DepartureDateTime,
            request.Capacity,
            request.PricePerPerson,
            request.Description,
            request.EstimatedReturnDateTime);

        await _unitOfWork.Repository<Trip>().AddAsync(trip, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateTripResponse
        {
            TripId = trip.Id,
            Title = trip.Title,
            DepartureDateTime = trip.DepartureDateTime,
            Capacity = trip.Capacity
        };

        return Result<CreateTripResponse>.Success(response, "Trip created successfully.");
    }
}