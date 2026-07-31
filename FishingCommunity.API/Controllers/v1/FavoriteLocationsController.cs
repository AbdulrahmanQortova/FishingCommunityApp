using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Map.FavoriteLocations.Commands.ToggleFavoriteLocation;
using FishingCommunity.Application.Features.Map.FavoriteLocations.Queries.GetMyFavoriteLocations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/favorite-locations")]
[Authorize]
public class FavoriteLocationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public FavoriteLocationsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyFavoriteLocationsQuery { UserId = _currentUserService.UserId!.Value }, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{spotId:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid spotId, CancellationToken cancellationToken)
    {
        var command = new ToggleFavoriteLocationCommand { UserId = _currentUserService.UserId!.Value, FishingSpotId = spotId };
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}