using FishingCommunity.Domain.Entities.Identity;

namespace FishingCommunity.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, IList<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateAccessTokenAndGetUserId(string token);
}