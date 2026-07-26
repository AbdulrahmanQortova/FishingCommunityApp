using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.Extensions.Options;
using RefreshTokenEntity = FishingCommunity.Domain.Entities.Identity.RefreshToken;

namespace FishingCommunity.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly FeatureFlags _featureFlags;

    private const int RefreshTokenValidDays = 7;
    private const int AccessTokenValidMinutes = 15;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IOptions<FeatureFlags> featureFlags)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _featureFlags = featureFlags.Value;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, userId, roles) = await _identityService.ValidateCredentialsAsync(
            request.Email, request.Password, cancellationToken);

        if (!succeeded || userId is null)
        {
            return Result<LoginResponse>.Failure("Invalid email or password.");
        }

        var profile = await _identityService.GetUserProfileAsync(userId.Value, cancellationToken);

        if (profile is null)
        {
            return Result<LoginResponse>.Failure("User profile could not be retrieved.");
        }

        // Email verification check is now controlled by a feature flag —
        // disable it in appsettings.json (FeatureFlags:RequireEmailVerification) during
        // development or if SMTP isn't configured yet.
        if (_featureFlags.RequireEmailVerification && !profile.IsEmailVerified)
        {
            return Result<LoginResponse>.Failure("Please verify your email before logging in.");
        }

        var accessToken = _jwtTokenService.GenerateAccessToken(userId.Value, profile.Email, roles);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshTokenEntity(
            userId.Value,
            refreshTokenValue,
            _dateTimeProvider.UtcNow.AddDays(RefreshTokenValidDays),
            request.IpAddress ?? "unknown");

        await _unitOfWork.Repository<RefreshTokenEntity>().AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new LoginResponse
        {
            UserId = userId.Value,
            Email = profile.Email,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Roles = roles,
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiresOn = _dateTimeProvider.UtcNow.AddMinutes(AccessTokenValidMinutes)
        };

        return Result<LoginResponse>.Success(response, "Login successful.");
    }
}