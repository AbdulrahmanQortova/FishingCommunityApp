using FluentValidation;

namespace FishingCommunity.Application.Features.Map.FishingSpots.Commands.CreateFishingSpot;

public class CreateFishingSpotCommandValidator : AbstractValidator<CreateFishingSpotCommand>
{
    public CreateFishingSpotCommandValidator()
    {
        RuleFor(x => x.CreatedByUserId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Fishing spot name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.Type).IsInEnum();
    }
}