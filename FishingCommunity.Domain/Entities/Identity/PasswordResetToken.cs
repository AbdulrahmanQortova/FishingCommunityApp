using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Identity;

public class PasswordResetToken : BaseEntity
{
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresOn { get; private set; }
    public bool IsUsed { get; private set; }

    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    private PasswordResetToken() { }

    public PasswordResetToken(Guid userId, string token, DateTime expiresOn)
    {
        UserId = userId;
        Token = token;
        ExpiresOn = expiresOn;
    }

    public bool IsValid() => !IsUsed && DateTime.UtcNow < ExpiresOn;

    public void MarkAsUsed() => IsUsed = true;
}