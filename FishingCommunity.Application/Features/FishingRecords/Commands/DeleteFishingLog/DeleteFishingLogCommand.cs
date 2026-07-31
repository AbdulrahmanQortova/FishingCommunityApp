using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Commands.DeleteFishingLog;

public class DeleteFishingLogCommand : IRequest<Result>
{
    public Guid FishingLogId { get; set; }
    public Guid RequestingUserId { get; set; }
}