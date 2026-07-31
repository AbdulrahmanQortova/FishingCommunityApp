using FishingCommunity.Domain.Entities.FishingRecords;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Commands.UpdateFishingLog;

public class UpdateFishingLogCommandHandler : IRequestHandler<UpdateFishingLogCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFishingLogCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateFishingLogCommand request, CancellationToken cancellationToken)
    {
        var log = await _unitOfWork.Repository<FishingLog>().GetByIdAsync(request.FishingLogId, cancellationToken);

        if (log is null)
        {
            throw new NotFoundException(nameof(FishingLog), request.FishingLogId);
        }

        if (log.UserId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to update this fishing log.");
        }

        log.UpdateDetails(request.WeightKg, request.LengthCm, request.LocationName, request.Latitude, request.Longitude, request.Bait, request.Notes);

        _unitOfWork.Repository<FishingLog>().Update(log);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Fishing log updated successfully.");
    }
}