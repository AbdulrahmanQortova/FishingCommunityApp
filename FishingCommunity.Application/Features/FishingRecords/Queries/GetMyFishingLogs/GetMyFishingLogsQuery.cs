using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Queries.GetMyFishingLogs;

public class GetMyFishingLogsQuery : IRequest<Result<PaginatedList<FishingLogSummaryDto>>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? FishSpeciesId { get; set; } // Optional filter
}