using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Cart.Queries.GetMyCart;

public class GetMyCartQuery : IRequest<Result<CartDto>>
{
    public Guid UserId { get; set; }
}