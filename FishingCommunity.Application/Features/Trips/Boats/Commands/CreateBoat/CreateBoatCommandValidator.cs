using FluentValidation;

namespace FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;

public class CreateBoatCommandValidator : AbstractValidator<CreateBoatCommand>
{
    public CreateBoatCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Owner id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Boat name is required.")
            .MaximumLength(100).WithMessage("Boat name must not exceed 100 characters.");

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty().WithMessage("Registration number is required.")
            .MaximumLength(50).WithMessage("Registration number must not exceed 50 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Capacity seems unrealistically high — please verify.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => x.Description is not null);
    }
}