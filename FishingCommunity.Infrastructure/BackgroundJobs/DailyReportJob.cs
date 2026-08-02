using FishingCommunity.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FishingCommunity.Infrastructure.BackgroundJobs;

public class DailyReportJob
{
    private readonly Persistence.ApplicationDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly ILogger<DailyReportJob> _logger;

    public DailyReportJob(Persistence.ApplicationDbContext dbContext, IEmailService emailService, ILogger<DailyReportJob> logger)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("DailyReportJob started at {Time}", DateTime.UtcNow);

        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var today = DateTime.UtcNow.Date;

        var newUsersCount = await _dbContext.Users.CountAsync(u => u.CreatedDate >= yesterday && u.CreatedDate < today);
        var newOrdersCount = await _dbContext.Orders.CountAsync(o => o.CreatedDate >= yesterday && o.CreatedDate < today);
        var newTripsCount = await _dbContext.Trips.CountAsync(t => t.CreatedDate >= yesterday && t.CreatedDate < today);
        var newBookingsCount = await _dbContext.TripBookings.CountAsync(b => b.CreatedDate >= yesterday && b.CreatedDate < today);

        var revenueYesterday = await _dbContext.Orders
            .Where(o => o.CreatedDate >= yesterday && o.CreatedDate < today && o.Status != Domain.Enums.OrderStatus.Cancelled)
            .SumAsync(o => o.TotalAmount);

        var reportBody = $"""
            <h2>Daily Platform Report — {yesterday:yyyy-MM-dd}</h2>
            <ul>
                <li>New users: {newUsersCount}</li>
                <li>New orders: {newOrdersCount} (Revenue: {revenueYesterday:C})</li>
                <li>New trips created: {newTripsCount}</li>
                <li>New trip bookings: {newBookingsCount}</li>
            </ul>
            """;

        // TODO: Replace with the actual admin notification email(s) once configurable
        // via settings — hardcoded placeholder for now during development.
        var adminEmail = "admin@fishingcommunity.local";

        await _emailService.SendEmailAsync(adminEmail, $"Daily Report — {yesterday:yyyy-MM-dd}", reportBody);

        _logger.LogInformation("DailyReportJob completed and sent to {Email}.", adminEmail);
    }
}