using FluentValidation;

namespace FishingCommunity.Application.Features.Shop.Products.Commands.AddProductReview;

public class AddProductReviewCommandValidator : AbstractValidator<AddProductReviewCommand>
{
    public AddProductReviewCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(1000).When(x => x.Comment is not null);
    }
}