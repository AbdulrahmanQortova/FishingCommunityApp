using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.FishingRecords;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.FishingRecords.Queries.GetMyFishingLogs;

public class GetMyFishingLogsQueryHandler : IRequestHandler<GetMyFishingLogsQuery, Result<PaginatedList<FishingLogSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMyFishingLogsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<FishingLogSummaryDto>>> Handle(GetMyFishingLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<FishingLog>().Query()
            .Where(l => l.UserId == request.UserId);

        if (request.FishSpeciesId.HasValue)
        {
            query = query.Where(l => l.FishSpeciesId == request.FishSpeciesId.Value);
        }

        var projectedQuery = query
            .OrderByDescending(l => l.CaughtDate)
            .Select(l => new FishingLogSummaryDto
            {
                Id = l.Id,
                FishSpeciesName = l.FishSpecies.Name,
                WeightKg = l.WeightKg,
                LengthCm = l.LengthCm,
                LocationName = l.LocationName,
                CaughtDate = l.CaughtDate,
                MainPhotoUrl = l.PhotoUrls.FirstOrDefault()
            });

        var result = await projectedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<FishingLogSummaryDto>>.Success(result);
    }
}