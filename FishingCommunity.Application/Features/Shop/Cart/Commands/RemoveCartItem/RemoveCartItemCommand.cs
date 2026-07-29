using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
}