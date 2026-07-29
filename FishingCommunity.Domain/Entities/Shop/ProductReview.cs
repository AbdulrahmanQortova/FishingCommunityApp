using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class ProductReview : BaseAuditableEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    private ProductReview() { } // EF Core

    internal ProductReview(Guid productId, Guid userId, int rating, string? comment)
    {
        if (rating is < 1 or > 5)
        {
            throw new BusinessRuleValidationException("Rating must be between 1 and 5.");
        }

        ProductId = productId;
        UserId = userId;
        Rating = rating;
        Comment = comment;
    }
}