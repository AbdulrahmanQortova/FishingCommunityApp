using FluentValidation;

namespace FishingCommunity.Application.Features.Weather.Queries.GetWeatherForLocation;

public class GetWeatherForLocationQueryValidator : AbstractValidator<GetWeatherForLocationQuery>
{
    public GetWeatherForLocationQueryValidator()
    {
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}