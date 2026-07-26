using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Identity;

public class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresOn { get; private set; }
    public DateTime CreatedOn { get; private set; }

    public string? RevokedByIp { get; private set; }
    public DateTime? RevokedOn { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public string CreatedByIp { get; private set; } = string.Empty;

    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsRevoked => RevokedOn != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken() { } // For EF Core

    public RefreshToken(Guid userId, string token, DateTime expiresOn, string createdByIp)
    {
        UserId = userId;
        Token = token;
        ExpiresOn = expiresOn;
        CreatedByIp = createdByIp;
        CreatedOn = DateTime.UtcNow;
    }

    public void Revoke(string revokedByIp, string? replacedByToken = null)
    {
        RevokedOn = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }
}