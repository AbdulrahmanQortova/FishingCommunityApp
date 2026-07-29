using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Community.Follows.Commands.FollowUser;
using FishingCommunity.Application.Features.Community.Follows.Commands.UnfollowUser;
using FishingCommunity.Application.Features.Community.Follows.Queries.GetFollowers;
using FishingCommunity.Application.Features.Community.Follows.Queries.GetFollowing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users/{userId:guid}")]
public class FollowsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public FollowsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost("follow")]
    [Authorize]
    public async Task<IActionResult> Follow(Guid userId, CancellationToken cancellationToken)
    {
        var command = new FollowUserCommand
        {
            FollowerId = _currentUserService.UserId!.Value,
            FollowedId = userId
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("follow")]
    [Authorize]
    public async Task<IActionResult> Unfollow(Guid userId, CancellationToken cancellationToken)
    {
        var command = new UnfollowUserCommand
        {
            FollowerId = _currentUserService.UserId!.Value,
            FollowedId = userId
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("followers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFollowers(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetFollowersQuery { UserId = userId }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("following")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFollowing(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetFollowingQuery { UserId = userId }, cancellationToken);
        return Ok(result);
    }
}