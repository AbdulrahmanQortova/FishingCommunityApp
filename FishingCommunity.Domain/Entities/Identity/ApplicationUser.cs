using FishingCommunity.Domain.Common;
using FishingCommunity.Domain.Enums;
using Microsoft.AspNetCore.Identity;  

namespace FishingCommunity.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public AccountStatus Status { get; set; } = AccountStatus.PendingVerification;

    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }

    public DateTime CreatedDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}