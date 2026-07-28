using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Trips.Bookings.Commands.ApproveBooking;
using FishingCommunity.Application.Features.Trips.Bookings.Commands.CancelBooking;
using FishingCommunity.Application.Features.Trips.Bookings.Commands.CheckInBooking;
using FishingCommunity.Application.Features.Trips.Bookings.Commands.RejectBooking;
using FishingCommunity.Application.Features.Trips.Bookings.Commands.RequestBooking;
using FishingCommunity.Application.Features.Trips.Bookings.Queries.GetMyBookings;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/trips/{tripId:guid}/bookings")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public BookingsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> RequestBooking(Guid tripId, [FromBody] RequestBookingRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RequestBookingCommand
        {
            TripId = tripId,
            UserId = _currentUserService.UserId!.Value,
            SeatsRequested = request.SeatsRequested
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{bookingId:guid}/approve")]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> Approve(Guid tripId, Guid bookingId, CancellationToken cancellationToken)
    {
        var command = new ApproveBookingCommand
        {
            TripId = tripId,
            BookingId = bookingId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{bookingId:guid}/reject")]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> Reject(Guid tripId, Guid bookingId, [FromBody] RejectBookingRequestDto request, CancellationToken cancellationToken)
    {
        var command = new RejectBookingCommand
        {
            TripId = tripId,
            BookingId = bookingId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Reason = request.Reason
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{bookingId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid tripId, Guid bookingId, CancellationToken cancellationToken)
    {
        var command = new CancelBookingCommand
        {
            TripId = tripId,
            BookingId = bookingId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("/api/v{version:apiVersion}/bookings/mine")]
    public async Task<IActionResult> GetMyBookings(CancellationToken cancellationToken)
    {
        var query = new GetMyBookingsQuery
        {
            UserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }
    [HttpPost("{bookingId:guid}/check-in")]
    [Authorize(Roles = Roles.BoatOwner)]
    public async Task<IActionResult> CheckIn(Guid tripId, Guid bookingId, CancellationToken cancellationToken)
    {
        var command = new CheckInBookingCommand
        {
            TripId = tripId,
            BookingId = bookingId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}