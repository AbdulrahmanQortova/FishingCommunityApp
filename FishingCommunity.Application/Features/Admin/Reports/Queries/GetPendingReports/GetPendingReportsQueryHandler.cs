using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Reports.Queries.GetPendingReports;

public class GetPendingReportsQueryHandler : IRequestHandler<GetPendingReportsQuery, Result<PaginatedList<PendingReportDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPendingReportsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<PendingReportDto>>> Handle(GetPendingReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PostReport>().Query()
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderByDescending(r => r.CreatedDate)
            .Select(r => new PendingReportDto
            {
                Id = r.Id,
                PostId = r.PostId,
                ReportedByUserId = r.ReportedByUserId,
                Reason = r.Reason,
                AdditionalDetails = r.AdditionalDetails,
                CreatedDate = r.CreatedDate
            });

        var result = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<PendingReportDto>>.Success(result);
    }
}