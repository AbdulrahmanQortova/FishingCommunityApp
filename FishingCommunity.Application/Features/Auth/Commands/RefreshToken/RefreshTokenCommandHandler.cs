using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using RefreshTokenEntity = FishingCommunity.Domain.Entities.Identity.RefreshToken;

namespace FishingCommunity.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    private const int RefreshTokenValidDays = 7;
    private const int AccessTokenValidMinutes = 15;

    public RefreshTokenCommandHandler(
        IJwtTokenService jwtTokenService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _jwtTokenService = jwtTokenService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = _jwtTokenService.GetUserIdFromExpiredToken(request.AccessToken);

        if (userId is null)
        {
            return Result<RefreshTokenResponse>.Failure("Invalid access token.");
        }

        var storedTokens = await _unitOfWork.Repository<RefreshTokenEntity>()
            .FindAsync(rt => rt.UserId == userId.Value && rt.Token == request.RefreshToken, cancellationToken);

        var existingToken = storedTokens.FirstOrDefault();

        if (existingToken is null)
        {
            return Result<RefreshTokenResponse>.Failure("Refresh token not found.");
        }

        if (!existingToken.IsActive)
        {
            return Result<RefreshTokenResponse>.Failure("Refresh token is expired or has been revoked.");
        }

        // Rotate: revoke the old refresh token and issue a brand new one (prevents reuse/replay attacks).
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        existingToken.Revoke(request.IpAddress ?? "unknown", newRefreshTokenValue);

        var newRefreshToken = new RefreshTokenEntity(
            userId.Value,
            newRefreshTokenValue,
            _dateTimeProvider.UtcNow.AddDays(RefreshTokenValidDays),
            request.IpAddress ?? "unknown");

        await _unitOfWork.Repository<RefreshTokenEntity>().AddAsync(newRefreshToken, cancellationToken);

        var roles = await _identityService.GetUserRolesAsync(userId.Value, cancellationToken);
        var profile = await _identityService.GetUserProfileAsync(userId.Value, cancellationToken);

        if (profile is null)
        {
            return Result<RefreshTokenResponse>.Failure("User no longer exists.");
        }

        var newAccessToken = _jwtTokenService.GenerateAccessToken(userId.Value, profile.Email, roles);

        _unitOfWork.Repository<RefreshTokenEntity>().Update(existingToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            AccessTokenExpiresOn = _dateTimeProvider.UtcNow.AddMinutes(AccessTokenValidMinutes)
        };

        return Result<RefreshTokenResponse>.Success(response);
    }
}