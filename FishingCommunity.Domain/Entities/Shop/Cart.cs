using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class Cart : BaseEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }

    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { } // EF Core

    public Cart(Guid userId)
    {
        UserId = userId;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPriceSnapshot)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleValidationException("Quantity must be greater than zero.");
        }

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + quantity);
            return;
        }

        _items.Add(new CartItem(Id, productId, quantity, unitPriceSnapshot));
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);

        if (item is null)
        {
            throw new Exceptions.NotFoundException(nameof(CartItem), productId);
        }

        if (quantity <= 0)
        {
            _items.Remove(item);
            return;
        }

        item.UpdateQuantity(quantity);
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            _items.Remove(item);
        }
    }

    public void Clear()
    {
        _items.Clear();
    }

    public decimal GetTotal() => _items.Sum(i => i.Quantity * i.UnitPriceSnapshot);
}