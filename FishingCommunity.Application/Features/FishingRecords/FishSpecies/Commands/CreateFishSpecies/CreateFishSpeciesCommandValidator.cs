using FluentValidation;

namespace FishingCommunity.Application.Features.FishingRecords.FishSpecies.Commands.CreateFishSpecies;

public class CreateFishSpeciesCommandValidator : AbstractValidator<CreateFishSpeciesCommand>
{
    public CreateFishSpeciesCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Fish species name is required.")
            .MaximumLength(100);

        RuleFor(x => x.ScientificName)
            .MaximumLength(150)
            .When(x => x.ScientificName is not null);
    }
}