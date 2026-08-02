namespace FishingCommunity.Application.Features.Admin.Analytics.Queries.GetDashboardStats;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalBoatOwners { get; set; }
    public int TotalStoreOwners { get; set; }

    public int TotalTrips { get; set; }
    public int UpcomingTrips { get; set; }

    public int TotalStores { get; set; }
    public int ActiveStores { get; set; }
    public int PendingStoreApprovals { get; set; }

    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }

    public int PendingReports { get; set; }
}