using FluentValidation;

namespace FishingCommunity.Application.Features.AiAssistant.Queries.GetEquipmentRecommendation;

public class GetEquipmentRecommendationQueryValidator : AbstractValidator<GetEquipmentRecommendationQuery>
{
    public GetEquipmentRecommendationQueryValidator()
    {
        RuleFor(x => x.TargetSpecies)
            .NotEmpty().WithMessage("Target species is required.");

        RuleFor(x => x.ExperienceLevel)
            .Must(level => new[] { "Beginner", "Intermediate", "Advanced" }.Contains(level))
            .WithMessage("Experience level must be Beginner, Intermediate, or Advanced.");
    }
}