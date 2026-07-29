using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Orders.Queries.GetOrderDetails;

public class GetOrderDetailsQuery : IRequest<Result<OrderDetailsDto>>
{
    public Guid OrderId { get; set; }
    public Guid RequestingUserId { get; set; }
}