using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.Community;
using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Constants;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Admin.Analytics.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var (allUsers, totalUsers) = await _identityService.GetUsersAsync(1, int.MaxValue, null, null, cancellationToken);

        var dto = new DashboardStatsDto
        {
            TotalUsers = totalUsers,
            TotalBoatOwners = allUsers.Count(u => u.Roles.Contains(Roles.BoatOwner)),
            TotalStoreOwners = allUsers.Count(u => u.Roles.Contains(Roles.StoreOwner)),

            TotalTrips = await _unitOfWork.Repository<Trip>().CountAsync(cancellationToken: cancellationToken),
            UpcomingTrips = await _unitOfWork.Repository<Trip>()
                .CountAsync(t => t.Status == TripStatus.Scheduled && t.DepartureDateTime > DateTime.UtcNow, cancellationToken),

            TotalStores = await _unitOfWork.Repository<Store>().CountAsync(cancellationToken: cancellationToken),
            ActiveStores = await _unitOfWork.Repository<Store>().CountAsync(s => s.Status == StoreStatus.Active, cancellationToken),
            PendingStoreApprovals = await _unitOfWork.Repository<Store>().CountAsync(s => s.Status == StoreStatus.UnderReview, cancellationToken),

            TotalOrders = await _unitOfWork.Repository<Order>().CountAsync(cancellationToken: cancellationToken),
            TotalRevenue = await GetTotalRevenueAsync(cancellationToken),

            PendingReports = await _unitOfWork.Repository<PostReport>().CountAsync(r => r.Status == ReportStatus.Pending, cancellationToken)
        };

        return Result<DashboardStatsDto>.Success(dto);
    }

    private async Task<decimal> GetTotalRevenueAsync(CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Order>().Query()
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => o.TotalAmount, cancellationToken);
    }
}