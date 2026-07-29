using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Exceptions;

namespace FishingCommunity.Domain.Entities.Shop;

public class Category : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    private Category() { } // EF Core

    public Category(string name, string? description = null, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Category name is required.");
        }

        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Category name is required.");
        }

        Name = name;
        Description = description;
    }
}