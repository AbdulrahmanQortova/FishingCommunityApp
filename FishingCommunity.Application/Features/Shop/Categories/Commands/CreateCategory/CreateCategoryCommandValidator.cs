using FluentValidation;

namespace FishingCommunity.Application.Features.Shop.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);

        RuleFor(x => x.ParentCategoryId)
            .NotEqual(Guid.Empty)
            .WithMessage("Parent category id cannot be an empty GUID — leave it null if this is a root category.")
            .When(x => x.ParentCategoryId.HasValue);
    }
}