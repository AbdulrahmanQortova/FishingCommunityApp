using FluentValidation;

namespace FishingCommunity.Application.Features.FishingRecords.Commands.CreateFishingLog;

public class CreateFishingLogCommandValidator : AbstractValidator<CreateFishingLogCommand>
{
    public CreateFishingLogCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FishSpeciesId).NotEmpty();

        RuleFor(x => x.CaughtDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Catch date cannot be in the future.");

        RuleFor(x => x.WeightKg)
            .GreaterThanOrEqualTo(0).When(x => x.WeightKg.HasValue);

        RuleFor(x => x.LengthCm)
            .GreaterThanOrEqualTo(0).When(x => x.LengthCm.HasValue);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(2000).When(x => x.Notes is not null);

        RuleFor(x => x.Bait)
            .MaximumLength(200).When(x => x.Bait is not null);

        RuleFor(x => x.PhotoUrls)
            .Must(urls => urls == null || urls.Count <= 10)
            .WithMessage("A fishing log cannot have more than 10 photos.");
    }
}