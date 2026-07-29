namespace FishingCommunity.Application.Features.Shop.Products.Queries.GetProducts;

public class ProductSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? MainPhotoUrl { get; set; }
    public double? AverageRating { get; set; }
    public bool InStock { get; set; }
    public string StoreName { get; set; } = string.Empty;
}