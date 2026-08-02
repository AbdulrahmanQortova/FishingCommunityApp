using FishingCommunity.Application.Common.Models;
using FishingCommunity.Shared.Wrappers;

namespace FishingCommunity.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<Guid>> CreateUserAsync(string email, string password, string firstName, string lastName, string role, CancellationToken cancellationToken = default);
    Task<Result> AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, Guid? UserId, IList<string> Roles)> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(Guid userId, string code, CancellationToken cancellationToken = default, bool bypassCodeCheck = false);
    Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetUserProfileByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateNewEmailVerificationCodeAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> UpdateProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        string? bio,
        DateTime? dateOfBirth,
        string? profilePictureUrl,
        CancellationToken cancellationToken = default);

    Task<(List<AdminUserListItemDto> Users, int TotalCount)> GetUsersAsync(
    int pageNumber, int pageSize, string? searchTerm, string? role, CancellationToken cancellationToken = default);

    Task<Result> SuspendUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> PromoteToAdminAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
}