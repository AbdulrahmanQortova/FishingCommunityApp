// Features/Shop/Products/Commands/UpdateProduct/UpdateProductRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.Products.Commands.UpdateProduct;

public class UpdateProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}