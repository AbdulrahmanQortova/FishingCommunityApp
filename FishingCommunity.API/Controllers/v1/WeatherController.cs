using Asp.Versioning;
using FishingCommunity.Application.Features.Weather.Queries.GetWeatherForLocation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/weather")]
[AllowAnonymous]
public class WeatherController : ControllerBase
{
    private readonly ISender _sender;

    public WeatherController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather([FromQuery] double latitude, [FromQuery] double longitude, CancellationToken cancellationToken)
    {
        var query = new GetWeatherForLocationQuery
        {
            Latitude = latitude,
            Longitude = longitude
        };

        var result = await _sender.Send(query, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}