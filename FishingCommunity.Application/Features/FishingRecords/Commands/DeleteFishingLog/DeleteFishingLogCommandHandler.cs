using FishingCommunity.Domain.Entities.FishingRecords;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Commands.DeleteFishingLog;

public class DeleteFishingLogCommandHandler : IRequestHandler<DeleteFishingLogCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFishingLogCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteFishingLogCommand request, CancellationToken cancellationToken)
    {
        var log = await _unitOfWork.Repository<FishingLog>().GetByIdAsync(request.FishingLogId, cancellationToken);

        if (log is null)
        {
            throw new NotFoundException(nameof(FishingLog), request.FishingLogId);
        }

        if (log.UserId != request.RequestingUserId)
        {
            return Result.Failure("You are not authorized to delete this fishing log.");
        }

        _unitOfWork.Repository<FishingLog>().Remove(log);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Fishing log deleted successfully.");
    }
}