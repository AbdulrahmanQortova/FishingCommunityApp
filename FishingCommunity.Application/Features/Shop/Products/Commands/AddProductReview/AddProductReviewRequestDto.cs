// Features/Shop/Products/Commands/AddProductReview/AddProductReviewRequestDto.cs
namespace FishingCommunity.Application.Features.Shop.Products.Commands.AddProductReview;

public class AddProductReviewRequestDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}