using Asp.Versioning;
using FishingCommunity.Application.Features.Admin.Users.Commands.PromoteToAdmin;
using FishingCommunity.Application.Features.Admin.Users.Commands.ReactivateUser;
using FishingCommunity.Application.Features.Admin.Users.Commands.SuspendUser;
using FishingCommunity.Application.Features.Admin.Users.Queries.GetAllUsers;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/users")]
[Authorize(Roles = Roles.Administrator)]
public class AdminUsersController : ControllerBase
{
    private readonly ISender _sender;

    public AdminUsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{userId:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new SuspendUserCommand { UserId = userId }, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{userId:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ReactivateUserCommand { UserId = userId }, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{userId:guid}/promote-to-admin")]
    public async Task<IActionResult> PromoteToAdmin(Guid userId, [FromBody] PromoteToAdminRequestDto request, CancellationToken cancellationToken)
    {
        // Extra confirmation step for this highly sensitive action — the client must
        // echo back a fixed confirmation phrase, reducing the chance of an accidental click.
        if (request.ConfirmationPhrase != "CONFIRM PROMOTE TO ADMIN")
        {
            return BadRequest(new { message = "Confirmation phrase does not match. Action aborted." });
        }

        var result = await _sender.Send(new PromoteToAdminCommand { UserId = userId }, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}