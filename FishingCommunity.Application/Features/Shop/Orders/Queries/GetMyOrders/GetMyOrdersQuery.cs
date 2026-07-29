using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Orders.Queries.GetMyOrders;

public class GetMyOrdersQuery : IRequest<Result<PaginatedList<MyOrderDto>>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}