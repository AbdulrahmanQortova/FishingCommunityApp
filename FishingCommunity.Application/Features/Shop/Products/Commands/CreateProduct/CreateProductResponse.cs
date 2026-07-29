namespace FishingCommunity.Application.Features.Shop.Products.Commands.CreateProduct;

public class CreateProductResponse
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}