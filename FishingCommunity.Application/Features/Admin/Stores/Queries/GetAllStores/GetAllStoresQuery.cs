using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Stores.Queries.GetAllStores;

public class GetAllStoresQuery : IRequest<Result<PaginatedList<AdminStoreDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? StatusFilter { get; set; } // "UnderReview", "Active", "Suspended", "Closed"
}