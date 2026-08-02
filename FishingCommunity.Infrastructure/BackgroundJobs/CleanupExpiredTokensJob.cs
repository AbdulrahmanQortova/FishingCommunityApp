using FishingCommunity.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FishingCommunity.Infrastructure.BackgroundJobs;

public class CleanupExpiredTokensJob
{
    private readonly Persistence.ApplicationDbContext _dbContext;
    private readonly ILogger<CleanupExpiredTokensJob> _logger;

    public CleanupExpiredTokensJob(Persistence.ApplicationDbContext dbContext, ILogger<CleanupExpiredTokensJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("CleanupExpiredTokensJob started at {Time}", DateTime.UtcNow);

        var now = DateTime.UtcNow;
        var retentionCutoff = now.AddDays(-30); // Keep expired tokens for 30 days for audit purposes, then hard-delete.

        var expiredRefreshTokens = await _dbContext.RefreshTokens
            .Where(rt => rt.ExpiresOn < retentionCutoff || (rt.RevokedOn != null && rt.RevokedOn < retentionCutoff))
            .ToListAsync();

        var expiredEmailTokens = await _dbContext.EmailVerificationTokens
            .Where(t => t.ExpiresOn < retentionCutoff)
            .ToListAsync();

        var expiredResetTokens = await _dbContext.PasswordResetTokens
            .Where(t => t.ExpiresOn < retentionCutoff)
            .ToListAsync();

        _dbContext.RefreshTokens.RemoveRange(expiredRefreshTokens);
        _dbContext.EmailVerificationTokens.RemoveRange(expiredEmailTokens);
        _dbContext.PasswordResetTokens.RemoveRange(expiredResetTokens);

        var totalDeleted = expiredRefreshTokens.Count + expiredEmailTokens.Count + expiredResetTokens.Count;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("CleanupExpiredTokensJob completed. Deleted {Count} expired tokens.", totalDeleted);
    }
}