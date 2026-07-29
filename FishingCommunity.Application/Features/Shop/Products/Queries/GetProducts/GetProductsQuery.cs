using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<Result<PaginatedList<ProductSummaryDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public Guid? CategoryId { get; set; }
    public Guid? StoreId { get; set; }
    public string? SearchTerm { get; set; }
    public decimal? MaxPrice { get; set; }
}