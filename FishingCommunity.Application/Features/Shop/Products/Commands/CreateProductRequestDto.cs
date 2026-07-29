// Features/Shop/Products/Commands/CreateProduct/CreateProductRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.Products.Commands.CreateProduct;

public class CreateProductRequestDto
{
    public Guid StoreId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}