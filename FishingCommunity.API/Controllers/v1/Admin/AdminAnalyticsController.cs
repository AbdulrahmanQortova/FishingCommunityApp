using Asp.Versioning;
using FishingCommunity.Application.Features.Admin.Analytics.Queries.GetDashboardStats;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/analytics")]
[Authorize(Roles = Roles.Administrator)]
public class AdminAnalyticsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminAnalyticsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDashboardStatsQuery(), cancellationToken);
        return Ok(result);
    }
}