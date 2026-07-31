using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.FishingRecords.Commands.CreateFishingLog;
using FishingCommunity.Application.Features.FishingRecords.Commands.DeleteFishingLog;
using FishingCommunity.Application.Features.FishingRecords.Commands.UpdateFishingLog;
using FishingCommunity.Application.Features.FishingRecords.Queries.GetFishingLogDetails;
using FishingCommunity.Application.Features.FishingRecords.Queries.GetMyFishingLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/fishing-logs")]
[Authorize]
public class FishingLogsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public FishingLogsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyLogs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? fishSpeciesId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyFishingLogsQuery
        {
            UserId = _currentUserService.UserId!.Value,
            PageNumber = pageNumber,
            PageSize = pageSize,
            FishSpeciesId = fishSpeciesId
        };

        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{logId:guid}")]
    public async Task<IActionResult> GetDetails(Guid logId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetFishingLogDetailsQuery { FishingLogId = logId }, cancellationToken);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFishingLogRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateFishingLogCommand
        {
            UserId = _currentUserService.UserId!.Value,
            FishSpeciesId = request.FishSpeciesId,
            CaughtDate = request.CaughtDate,
            WeightKg = request.WeightKg,
            LengthCm = request.LengthCm,
            LocationName = request.LocationName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Bait = request.Bait,
            Notes = request.Notes,
            PhotoUrls = request.PhotoUrls,
            CaptureWeather = request.CaptureWeather
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{logId:guid}")]
    public async Task<IActionResult> Update(Guid logId, [FromBody] UpdateFishingLogRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateFishingLogCommand
        {
            FishingLogId = logId,
            RequestingUserId = _currentUserService.UserId!.Value,
            WeightKg = request.WeightKg,
            LengthCm = request.LengthCm,
            LocationName = request.LocationName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Bait = request.Bait,
            Notes = request.Notes
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{logId:guid}")]
    public async Task<IActionResult> Delete(Guid logId, CancellationToken cancellationToken)
    {
        var command = new DeleteFishingLogCommand
        {
            FishingLogId = logId,
            RequestingUserId = _currentUserService.UserId!.Value
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}