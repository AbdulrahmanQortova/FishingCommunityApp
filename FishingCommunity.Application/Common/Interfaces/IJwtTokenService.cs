namespace FishingCommunity.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, IList<string> roles);
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a token that must still be within its expiry window.
    /// Used for normal authenticated requests.
    /// </summary>
    Guid? ValidateAccessTokenAndGetUserId(string token);

    /// <summary>
    /// Extracts the user id from an access token's claims even if the token has expired,
    /// as long as the signature is valid. Used only during the refresh-token flow.
    /// </summary>
    Guid? GetUserIdFromExpiredToken(string expiredAccessToken);
}