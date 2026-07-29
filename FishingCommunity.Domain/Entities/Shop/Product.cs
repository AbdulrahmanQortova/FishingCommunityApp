using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class Product : BaseAuditableEntity, IAggregateRoot
{
    public Guid StoreId { get; private set; }
    public Store Store { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string? MainPhotoUrl { get; private set; }

    public ProductStatus Status { get; private set; } = ProductStatus.Active;

    private readonly List<string> _photoUrls = new();
    public IReadOnlyCollection<string> PhotoUrls => _photoUrls.AsReadOnly();

    private readonly List<ProductReview> _reviews = new();
    public IReadOnlyCollection<ProductReview> Reviews => _reviews.AsReadOnly();

    public double? AverageRating => _reviews.Count > 0 ? _reviews.Average(r => r.Rating) : null;

    private Product() { } // EF Core

    public Product(Guid storeId, Guid categoryId, string name, decimal price, int stockQuantity, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Product name is required.");
        }

        if (price < 0)
        {
            throw new BusinessRuleValidationException("Product price cannot be negative.");
        }

        if (stockQuantity < 0)
        {
            throw new BusinessRuleValidationException("Stock quantity cannot be negative.");
        }

        StoreId = storeId;
        CategoryId = categoryId;
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
        Description = description;

        if (stockQuantity == 0)
        {
            Status = ProductStatus.OutOfStock;
        }
    }

    public void UpdateDetails(string name, string? description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Product name is required.");
        }

        if (price < 0)
        {
            throw new BusinessRuleValidationException("Product price cannot be negative.");
        }

        Name = name;
        Description = description;
        Price = price;
    }

    public void AddPhoto(string url) => _photoUrls.Add(url);
    public void RemovePhoto(string url) => _photoUrls.Remove(url);
    public void SetMainPhoto(string url) => MainPhotoUrl = url;

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleValidationException("Quantity to add must be greater than zero.");
        }

        StockQuantity += quantity;

        if (Status == ProductStatus.OutOfStock && StockQuantity > 0)
        {
            Status = ProductStatus.Active;
        }
    }

    public void ReserveStock(int quantity)
    {
        if (Status != ProductStatus.Active)
        {
            throw new BusinessRuleValidationException("This product is not currently available for purchase.");
        }

        if (quantity > StockQuantity)
        {
            throw new BusinessRuleValidationException($"Insufficient stock. Only {StockQuantity} units available.");
        }

        StockQuantity -= quantity;

        if (StockQuantity == 0)
        {
            Status = ProductStatus.OutOfStock;
        }
    }

    public void RestoreStock(int quantity)
    {
        // Used when an order is cancelled/refunded — release the reserved stock back.
        StockQuantity += quantity;

        if (Status == ProductStatus.OutOfStock && StockQuantity > 0)
        {
            Status = ProductStatus.Active;
        }
    }

    public void Discontinue() => Status = ProductStatus.Discontinued;

    public ProductReview AddReview(Guid userId, int rating, string? comment)
    {
        if (rating is < 1 or > 5)
        {
            throw new BusinessRuleValidationException("Rating must be between 1 and 5.");
        }

        if (_reviews.Any(r => r.UserId == userId))
        {
            throw new BusinessRuleValidationException("You have already reviewed this product.");
        }

        var review = new ProductReview(Id, userId, rating, comment);
        _reviews.Add(review);

        return review;
    }
}