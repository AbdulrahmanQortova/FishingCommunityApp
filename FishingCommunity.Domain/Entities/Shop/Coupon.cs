using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class Coupon : BaseAuditableEntity
{
    public string Code { get; private set; } = string.Empty;
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; } // Percentage (0-100) or fixed amount
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public int? MaxUsageCount { get; private set; } // null = unlimited
    public int UsageCount { get; private set; }
    public decimal? MinOrderAmount { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Coupon() { } // EF Core

    public Coupon(string code, DiscountType type, decimal value, DateTime validFrom, DateTime validTo, int? maxUsageCount = null, decimal? minOrderAmount = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BusinessRuleValidationException("Coupon code is required.");
        }

        if (type == DiscountType.Percentage && (value <= 0 || value > 100))
        {
            throw new BusinessRuleValidationException("Percentage discount must be between 0 and 100.");
        }

        if (type == DiscountType.FixedAmount && value <= 0)
        {
            throw new BusinessRuleValidationException("Fixed discount amount must be greater than zero.");
        }

        if (validTo <= validFrom)
        {
            throw new BusinessRuleValidationException("Coupon end date must be after the start date.");
        }

        Code = code.ToUpperInvariant();
        Type = type;
        Value = value;
        ValidFrom = validFrom;
        ValidTo = validTo;
        MaxUsageCount = maxUsageCount;
        MinOrderAmount = minOrderAmount;
    }

    public bool IsValidForUse(decimal orderAmount)
    {
        if (!IsActive) return false;
        if (DateTime.UtcNow < ValidFrom || DateTime.UtcNow > ValidTo) return false;
        if (MaxUsageCount.HasValue && UsageCount >= MaxUsageCount.Value) return false;
        if (MinOrderAmount.HasValue && orderAmount < MinOrderAmount.Value) return false;

        return true;
    }

    public decimal CalculateDiscount(decimal orderAmount)
    {
        return Type == DiscountType.Percentage
            ? orderAmount * (Value / 100m)
            : Math.Min(Value, orderAmount); // Never discount more than the order total.
    }

    public void RecordUsage()
    {
        UsageCount++;
    }

    public void Deactivate() => IsActive = false;
}