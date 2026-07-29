using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class Store : BaseAuditableEntity, IAggregateRoot
{
    public Guid OwnerId { get; private set; } // FK to ApplicationUser (StoreOwner role)

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? BannerUrl { get; private set; }

    public StoreStatus Status { get; private set; } = StoreStatus.UnderReview;

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Store() { } // EF Core

    public Store(Guid ownerId, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Store name is required.");
        }

        OwnerId = ownerId;
        Name = name;
        Description = description;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Store name is required.");
        }

        Name = name;
        Description = description;
    }

    public void SetLogo(string url) => LogoUrl = url;
    public void SetBanner(string url) => BannerUrl = url;

    public void Approve() => Status = StoreStatus.Active;
    public void Suspend() => Status = StoreStatus.Suspended;
    public void Close() => Status = StoreStatus.Closed;

    public void EnsureCanSell()
    {
        if (Status != StoreStatus.Active)
        {
            throw new BusinessRuleValidationException("This store is not currently active and cannot list or sell products.");
        }
    }
}