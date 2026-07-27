using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.UpdateBoat;

public class UpdateBoatCommandHandler : IRequestHandler<UpdateBoatCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBoatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateBoatCommand request, CancellationToken cancellationToken)
    {
        var boat = await _unitOfWork.Repository<Boat>().GetByIdAsync(request.BoatId, cancellationToken);

        if (boat is null)
        {
            throw new NotFoundException(nameof(Boat), request.BoatId);
        }

        if (boat.OwnerId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to update this boat.");
        }

        boat.UpdateDetails(request.Name, request.Description, request.Capacity);

        _unitOfWork.Repository<Boat>().Update(boat);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Boat updated successfully.");
    }
}