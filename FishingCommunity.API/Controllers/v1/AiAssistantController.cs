using Asp.Versioning;
using FishingCommunity.Application.Features.AiAssistant.Queries.GetEquipmentRecommendation;
using FishingCommunity.Application.Features.AiAssistant.Queries.GetFishingRecommendation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ai-assistant")]
[Authorize]
public class AiAssistantController : ControllerBase
{
    private readonly ISender _sender;

    public AiAssistantController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("fishing-recommendation")]
    public async Task<IActionResult> GetFishingRecommendation(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] string? preferredFishSpecies,
        CancellationToken cancellationToken)
    {
        var query = new GetFishingRecommendationQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            PreferredFishSpecies = preferredFishSpecies
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("equipment-recommendation")]
    public async Task<IActionResult> GetEquipmentRecommendation([FromQuery] GetEquipmentRecommendationQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}