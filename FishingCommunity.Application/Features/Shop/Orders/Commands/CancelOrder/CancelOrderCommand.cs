using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<Result>
{
    public Guid OrderId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string? Reason { get; set; }
}