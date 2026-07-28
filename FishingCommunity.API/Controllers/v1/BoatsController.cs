using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Trips.Boats.Commands.CreateBoat;
using FishingCommunity.Application.Features.Trips.Boats.Commands.DeleteBoat;
using FishingCommunity.Application.Features.Trips.Boats.Commands.UpdateBoat;
using FishingCommunity.Application.Features.Trips.Boats.Queries.GetMyBoats;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/boats")]
[Authorize(Roles = Roles.BoatOwner)]
public class BoatsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public BoatsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBoatRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateBoatCommand
        {
            OwnerId = _currentUserService.UserId!.Value,
            Name = request.Name,
            Description = request.Description,
            RegistrationNumber = request.RegistrationNumber,
            Capacity = request.Capacity
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{boatId:guid}")]
    public async Task<IActionResult> Update(Guid boatId, [FromBody] UpdateBoatRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateBoatCommand
        {
            BoatId = boatId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Name = request.Name,
            Description = request.Description,
            Capacity = request.Capacity
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{boatId:guid}")]
    public async Task<IActionResult> Delete(Guid boatId, CancellationToken cancellationToken)
    {
        var command = new DeleteBoatCommand
        {
            BoatId = boatId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyBoats(CancellationToken cancellationToken)
    {
        var query = new GetMyBoatsQuery
        {
            OwnerId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }
}