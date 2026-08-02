using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Analytics.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<Result<DashboardStatsDto>>
{
}