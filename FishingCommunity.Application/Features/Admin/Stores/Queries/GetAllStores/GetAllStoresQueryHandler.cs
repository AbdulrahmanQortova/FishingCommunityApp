using FishingCommunity.Application.Common.Extensions;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Stores.Queries.GetAllStores;

public class GetAllStoresQueryHandler : IRequestHandler<GetAllStoresQuery, Result<PaginatedList<AdminStoreDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllStoresQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<AdminStoreDto>>> Handle(GetAllStoresQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Store>().Query();

        if (!string.IsNullOrWhiteSpace(request.StatusFilter) && Enum.TryParse<StoreStatus>(request.StatusFilter, out var status))
        {
            query = query.Where(s => s.Status == status);
        }

        var projectedQuery = query
            .OrderByDescending(s => s.CreatedDate)
            .Select(s => new AdminStoreDto
            {
                Id = s.Id,
                Name = s.Name,
                OwnerId = s.OwnerId,
                Status = s.Status,
                ProductsCount = s.Products.Count,
                CreatedDate = s.CreatedDate
            });

        var result = await projectedQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        return Result<PaginatedList<AdminStoreDto>>.Success(result);
    }
}