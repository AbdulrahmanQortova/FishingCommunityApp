using FishingCommunity.Domain.Common;

namespace FishingCommunity.Domain.Entities.Identity;

public class EmailVerificationToken : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public DateTime ExpiresOn { get; private set; }
    public bool IsUsed { get; private set; }

    public Guid UserId { get; private set; }
    public ApplicationUser User { get; private set; } = null!;

    private EmailVerificationToken() { }

    public EmailVerificationToken(Guid userId, string code, DateTime expiresOn)
    {
        UserId = userId;
        Code = code;
        ExpiresOn = expiresOn;
    }

    public bool IsValid() => !IsUsed && DateTime.UtcNow < ExpiresOn;

    public void MarkAsUsed() => IsUsed = true;
}