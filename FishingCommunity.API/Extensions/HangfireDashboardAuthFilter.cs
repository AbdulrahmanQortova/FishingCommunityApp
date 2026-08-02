using Hangfire.Dashboard;

namespace FishingCommunity.API.Extensions;

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Only authenticated users with the Administrator role can access the dashboard.
        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.IsInRole(FishingCommunity.Shared.Constants.Roles.Administrator);
    }
}