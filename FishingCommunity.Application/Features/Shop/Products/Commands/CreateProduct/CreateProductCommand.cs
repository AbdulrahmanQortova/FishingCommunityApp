using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Result<CreateProductResponse>>
{
    public Guid StoreId { get; set; }
    public Guid RequestingUserId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}