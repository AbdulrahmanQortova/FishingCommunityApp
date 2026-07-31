using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Map.FishingSpots.Commands.CreateFishingSpot;
using FishingCommunity.Application.Features.Map.FishingSpots.Commands.VerifyFishingSpot;
using FishingCommunity.Application.Features.Map.FishingSpots.Queries.GetNearbyFishingSpots;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/fishing-spots")]
public class FishingSpotsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public FishingSpotsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet("nearby")]
    [AllowAnonymous]
    public async Task<IActionResult> GetNearby([FromQuery] GetNearbyFishingSpotsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateFishingSpotRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateFishingSpotCommand
        {
            CreatedByUserId = _currentUserService.UserId!.Value,
            Name = request.Name,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Type = request.Type
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{spotId:guid}/verify")]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> Verify(Guid spotId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new VerifyFishingSpotCommand { FishingSpotId = spotId }, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}