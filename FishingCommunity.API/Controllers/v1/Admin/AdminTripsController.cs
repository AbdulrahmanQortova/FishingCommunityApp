using Asp.Versioning;
using FishingCommunity.Application.Features.Admin.Trips.Commands.AdminCancelTrip;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/trips")]
[Authorize(Roles = Roles.Administrator)]
public class AdminTripsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminTripsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{tripId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid tripId, [FromBody] AdminCancelTripRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AdminCancelTripCommand { TripId = tripId, Reason = request.Reason };
        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}