using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.Trips;
using FishingCommunity.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FishingCommunity.Infrastructure.BackgroundJobs;

public class TripReminderJob
{
    private readonly Persistence.ApplicationDbContext _dbContext;
    private readonly INotificationService _notificationService;
    private readonly ILogger<TripReminderJob> _logger;

    public TripReminderJob(
        Persistence.ApplicationDbContext dbContext,
        INotificationService notificationService,
        ILogger<TripReminderJob> logger)
    {
        _dbContext = dbContext;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("TripReminderJob started at {Time}", DateTime.UtcNow);

        // Find trips departing within the next 24 hours that haven't been reminded yet.
        var reminderWindowStart = DateTime.UtcNow;
        var reminderWindowEnd = DateTime.UtcNow.AddHours(24);

        var upcomingTrips = await _dbContext.Trips
            .Where(t =>
                t.Status == TripStatus.Scheduled &&
                t.DepartureDateTime >= reminderWindowStart &&
                t.DepartureDateTime <= reminderWindowEnd)
            .Include(t => t.Bookings)
            .ToListAsync();

        var remindedCount = 0;

        foreach (var trip in upcomingTrips)
        {
            var confirmedParticipants = trip.Bookings
                .Where(b => b.Status is BookingStatus.Approved or BookingStatus.CheckedIn)
                .Select(b => b.UserId)
                .Distinct();

            foreach (var userId in confirmedParticipants)
            {
                await _notificationService.CreateNotificationAsync(
                    userId,
                    NotificationType.System,
                    "Trip reminder",
                    $"Your trip \"{trip.Title}\" departs in less than 24 hours. Don't forget your gear!",
                    trip.Id);

                remindedCount++;
            }
        }

        _logger.LogInformation("TripReminderJob completed. Sent {Count} reminders for {TripCount} trips.", remindedCount, upcomingTrips.Count);
    }
}