using FluentValidation;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.UpdateBoat;

public class UpdateBoatCommandValidator : AbstractValidator<UpdateBoatCommand>
{
    public UpdateBoatCommandValidator()
    {
        RuleFor(x => x.BoatId).NotEmpty();
        RuleFor(x => x.RequestingUserId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Boat name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.")
            .LessThanOrEqualTo(100);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);
    }
}