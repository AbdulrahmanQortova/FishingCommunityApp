using FishingCommunity.Domain.Entities.Identity;

namespace FishingCommunity.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateAccessTokenAndGetUserId(string token);
}