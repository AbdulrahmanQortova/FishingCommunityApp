using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Trips.Commands.CancelTrip;
using FishingCommunity.Application.Features.Trips.Commands.CompleteTrip;
using FishingCommunity.Application.Features.Trips.Commands.CreateTrip;
using FishingCommunity.Application.Features.Trips.Commands.StartTrip;
using FishingCommunity.Application.Features.Trips.Commands.UpdateTrip;
using FishingCommunity.Application.Features.Trips.Queries.GetTripDetails;
using FishingCommunity.Application.Features.Trips.Queries.GetUpcomingTrips;
using FishingCommunity.Application.Features.Trips.Reviews.Commands.AddTripReview;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/trips")]
public class TripsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public TripsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetUpcoming(
        [FromQuery] GetUpcomingTripsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{tripId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDetails(Guid tripId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTripDetailsQuery { TripId = tripId }, cancellationToken);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> Create([FromBody] CreateTripRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateTripCommand
        {
            OrganizerId = _currentUserService.UserId!.Value,
            BoatId = request.BoatId,
            Title = request.Title,
            Description = request.Description,
            LocationName = request.LocationName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            DepartureDateTime = request.DepartureDateTime,
            EstimatedReturnDateTime = request.EstimatedReturnDateTime,
            Capacity = request.Capacity,
            PricePerPerson = request.PricePerPerson
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{tripId:guid}")]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> Update(Guid tripId, [FromBody] UpdateTripRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateTripCommand
        {
            TripId = tripId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Title = request.Title,
            Description = request.Description,
            DepartureDateTime = request.DepartureDateTime,
            EstimatedReturnDateTime = request.EstimatedReturnDateTime,
            PricePerPerson = request.PricePerPerson
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{tripId:guid}/cancel")]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> Cancel(Guid tripId, [FromBody] CancelTripRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CancelTripCommand
        {
            TripId = tripId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Reason = request.Reason
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{tripId:guid}/reviews")]
    [Authorize]
    public async Task<IActionResult> AddReview(Guid tripId, [FromBody] AddTripReviewRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddTripReviewCommand
        {
            TripId = tripId,
            UserId = _currentUserService.UserId!.Value,
            Rating = request.Rating,
            Comment = request.Comment
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{tripId:guid}/start")]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> Start(Guid tripId, CancellationToken cancellationToken)
    {
        var command = new StartTripCommand
        {
            TripId = tripId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{tripId:guid}/complete")]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> Complete(Guid tripId, CancellationToken cancellationToken)
    {
        var command = new CompleteTripCommand
        {
            TripId = tripId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}