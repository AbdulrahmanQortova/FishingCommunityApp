using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.DeleteBoat;

public class DeleteBoatCommandHandler : IRequestHandler<DeleteBoatCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBoatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBoatCommand request, CancellationToken cancellationToken)
    {
        var boat = await _unitOfWork.Repository<Boat>().GetByIdAsync(request.BoatId, cancellationToken);

        if (boat is null)
        {
            throw new NotFoundException(nameof(Boat), request.BoatId);
        }

        if (boat.OwnerId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to delete this boat.");
        }

        var hasActiveTrips = await _unitOfWork.Repository<Trip>()
            .AnyAsync(t => t.BoatId == request.BoatId && t.Status == TripStatus.Scheduled, cancellationToken);

        if (hasActiveTrips)
        {
            return Result.Failure("Cannot delete a boat that has scheduled trips. Cancel or complete them first.");
        }

        _unitOfWork.Repository<Boat>().Remove(boat); // Soft delete, handled by the interceptor.
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Boat deleted successfully.");
    }
}