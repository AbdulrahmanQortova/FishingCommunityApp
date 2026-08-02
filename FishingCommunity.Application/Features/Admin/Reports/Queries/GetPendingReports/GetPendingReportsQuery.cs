using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Reports.Queries.GetPendingReports;

public class GetPendingReportsQuery : IRequest<Result<PaginatedList<PendingReportDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}