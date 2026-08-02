using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Trips.Commands.AdminCancelTrip;

public class AdminCancelTripCommandHandler : IRequestHandler<AdminCancelTripCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminCancelTripCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AdminCancelTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _unitOfWork.Repository<Trip>().GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
        {
            throw new NotFoundException(nameof(Trip), request.TripId);
        }

        // Reuses the same Trip.Cancel() domain method as the organizer's own cancel flow —
        // no special "admin cancel" logic needed at the Domain level, since cancelling
        // is cancelling regardless of who triggered it. Only the authorization differs
        // (no OwnerId check here, since an admin can act on any trip).
        trip.Cancel(request.Reason);

        _unitOfWork.Repository<Trip>().Update(trip);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Trip cancelled by admin.");
    }
}