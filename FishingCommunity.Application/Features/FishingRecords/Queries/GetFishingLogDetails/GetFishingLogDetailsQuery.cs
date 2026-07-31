using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Queries.GetFishingLogDetails;

public class GetFishingLogDetailsQuery : IRequest<Result<FishingLogDetailsDto>>
{
    public Guid FishingLogId { get; set; }
}