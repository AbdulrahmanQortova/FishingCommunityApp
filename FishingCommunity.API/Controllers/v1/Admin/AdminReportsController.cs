using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Admin.Reports.Commands.ResolveReport;
using FishingCommunity.Application.Features.Admin.Reports.Queries.GetPendingReports;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/reports")]
[Authorize(Roles = Roles.Administrator)]
public class AdminReportsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public AdminReportsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending([FromQuery] GetPendingReportsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{reportId:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid reportId, [FromBody] ResolveReportRequestDto request, CancellationToken cancellationToken)
    {
        var command = new ResolveReportCommand
        {
            ReportId = reportId,
            AdminUserId = _currentUserService.UserId!.Value,
            NewStatus = request.NewStatus,
            ResolutionNotes = request.ResolutionNotes,
            DeletePost = request.DeletePost
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}