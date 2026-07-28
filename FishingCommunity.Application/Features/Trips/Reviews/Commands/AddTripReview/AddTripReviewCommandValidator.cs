using FluentValidation;

namespace FishingCommunity.Application.Features.Trips.Reviews.Commands.AddTripReview;

public class AddTripReviewCommandValidator : AbstractValidator<AddTripReviewCommand>
{
    public AddTripReviewCommandValidator()
    {
        RuleFor(x => x.TripId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .When(x => x.Comment is not null);
    }
}