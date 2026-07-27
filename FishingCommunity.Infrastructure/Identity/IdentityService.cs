using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Entities.Identity;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Utilities;
using FishingCommunity.Shared.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    private const int EmailVerificationCodeValidMinutes = 30;
    private const int PasswordResetTokenValidMinutes = 30;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> CreateUserAsync(
        string email, string password, string firstName, string lastName, string role,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            return Result<Guid>.Failure("An account with this email already exists.");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return Result<Guid>.Failure($"Role '{role}' does not exist.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Status = Domain.Enums.AccountStatus.PendingVerification,
            CreatedDate = _dateTimeProvider.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            return Result<Guid>.Failure(createResult.Errors.Select(e => e.Description));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            // Roll back user creation if role assignment fails, to avoid orphaned accounts with no role.
            await _userManager.DeleteAsync(user);
            return Result<Guid>.Failure(roleResult.Errors.Select(e => e.Description));
        }

        return Result<Guid>.Success(user.Id);
    }

    public async Task<Result> AssignRoleAsync(Guid userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return Result.Failure($"Role '{role}' does not exist.");
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            return Result.Success("User already has this role.");
        }

        var result = await _userManager.AddToRoleAsync(user, role);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Succeeded, Guid? UserId, IList<string> Roles)> ValidateCredentialsAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || user.IsDeleted)
        {
            return (false, null, new List<string>());
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return (false, null, new List<string>());
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            return (false, null, new List<string>());
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        user.LastLoginDate = _dateTimeProvider.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return (true, user.Id, roles);
    }

    public async Task<Result> ConfirmEmailAsync(Guid userId, string code, CancellationToken cancellationToken = default, bool bypassCodeCheck = false)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        if (user.IsEmailVerified)
        {
            return Result.Success("Email is already verified.");
        }

        if (!bypassCodeCheck)
        {
            var tokens = await _unitOfWork.Repository<EmailVerificationToken>()
                .FindAsync(t => t.UserId == userId && t.Code == code, cancellationToken);

            var token = tokens.FirstOrDefault();

            if (token is null || !token.IsValid())
            {
                return Result.Failure("Invalid or expired verification code.");
            }

            token.MarkAsUsed();
            _unitOfWork.Repository<EmailVerificationToken>().Update(token);
        }

        user.IsEmailVerified = true;
        user.EmailConfirmed = true;
        user.Status = Domain.Enums.AccountStatus.Active;
        await _userManager.UpdateAsync(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<string>> GenerateNewEmailVerificationCodeAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result<string>.Failure("User not found.");
        }

        var code = RandomCodeGenerator.GenerateNumericCode(6);

        var verificationToken = new EmailVerificationToken(
            userId,
            code,
            _dateTimeProvider.UtcNow.AddMinutes(EmailVerificationCodeValidMinutes));

        await _unitOfWork.Repository<EmailVerificationToken>().AddAsync(verificationToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(code);
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || user.IsDeleted)
        {
            return Result<string>.Failure("User not found.");
        }

        var token = RandomCodeGenerator.GenerateSecureToken();

        var resetToken = new PasswordResetToken(
            user.Id,
            token,
            _dateTimeProvider.UtcNow.AddMinutes(PasswordResetTokenValidMinutes));

        await _unitOfWork.Repository<PasswordResetToken>().AddAsync(resetToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(token);
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || user.IsDeleted)
        {
            return Result.Failure("Invalid request.");
        }

        var tokens = await _unitOfWork.Repository<PasswordResetToken>()
            .FindAsync(t => t.UserId == user.Id && t.Token == token, cancellationToken);

        var resetToken = tokens.FirstOrDefault();

        if (resetToken is null || !resetToken.IsValid())
        {
            return Result.Failure("Invalid or expired reset token.");
        }

        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
        {
            return Result.Failure(removePasswordResult.Errors.Select(e => e.Description));
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
        if (!addPasswordResult.Succeeded)
        {
            return Result.Failure(addPasswordResult.Errors.Select(e => e.Description));
        }

        resetToken.MarkAsUsed();
        _unitOfWork.Repository<PasswordResetToken>().Update(resetToken);

        // Security: revoke all active refresh tokens too, same as ChangePasswordAsync —
        // a password reset should invalidate any existing sessions, in case the account
        // was compromised and this reset is the legitimate owner regaining control.
        var activeTokens = await _unitOfWork.Repository<Domain.Entities.Identity.RefreshToken>()
            .FindAsync(rt => rt.UserId == user.Id && rt.RevokedOn == null && rt.ExpiresOn > DateTime.UtcNow, cancellationToken);

        foreach (var refreshToken in activeTokens)
        {
            refreshToken.Revoke("password-reset");
            _unitOfWork.Repository<Domain.Entities.Identity.RefreshToken>().Update(refreshToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            return Result.Failure(result.Errors.Select(e => e.Description));
        }

        // Security: revoke all active refresh tokens so any other logged-in session
        // is forced to re-authenticate after a password change.
        var activeTokens = await _unitOfWork.Repository<RefreshToken>()
            .FindAsync(rt => rt.UserId == userId && rt.RevokedOn == null && rt.ExpiresOn > DateTime.UtcNow, cancellationToken);

        foreach (var refreshToken in activeTokens)
        {
            refreshToken.Revoke("password-change");
            _unitOfWork.Repository<RefreshToken>().Update(refreshToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        // Soft delete: anonymize sensitive identifying fields but keep the row
        // (other entities like Posts/Orders/Trips may still reference this UserId).
        user.IsDeleted = true;
        user.DeletedDate = _dateTimeProvider.UtcNow;
        user.Status = Domain.Enums.AccountStatus.Deleted;
        user.Email = $"deleted_{userId}@fishingcommunity.local";
        user.NormalizedEmail = user.Email.ToUpperInvariant();
        user.UserName = $"deleted_{userId}";
        user.NormalizedUserName = user.UserName.ToUpperInvariant();
        user.PhoneNumber = null;
        user.ProfilePictureUrl = null;
        user.Bio = null;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    public async Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user is null
            ? new List<string>()
            : await _userManager.GetRolesAsync(user);
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user is null ? null : MapToProfileDto(user);
    }

    public async Task<UserProfileDto?> GetUserProfileByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : MapToProfileDto(user);
    }

    public async Task<Result> UpdateProfileAsync(
        Guid userId, string firstName, string lastName, string? bio, DateTime? dateOfBirth, string? profilePictureUrl,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        user.FirstName = firstName;
        user.LastName = lastName;
        user.Bio = bio;
        user.DateOfBirth = dateOfBirth;
        user.ProfilePictureUrl = profilePictureUrl;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.Description));
    }

    private static UserProfileDto MapToProfileDto(ApplicationUser user) => new()
    {
        UserId = user.Id,
        Email = user.Email ?? string.Empty,
        FirstName = user.FirstName,
        LastName = user.LastName,
        IsEmailVerified = user.IsEmailVerified
    };
}