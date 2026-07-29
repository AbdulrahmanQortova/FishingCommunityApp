using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<Result>
{
    public Guid ProductId { get; set; }
    public Guid RequestingUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}