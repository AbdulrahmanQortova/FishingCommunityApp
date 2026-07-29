using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Products.Commands.UpdateStock;

public class UpdateStockCommand : IRequest<Result>
{
    public Guid ProductId { get; set; }
    public Guid RequestingUserId { get; set; }
    public int QuantityToAdd { get; set; }
}