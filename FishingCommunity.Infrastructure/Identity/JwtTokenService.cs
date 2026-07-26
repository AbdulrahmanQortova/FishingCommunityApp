using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FishingCommunity.Infrastructure.Identity;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateAccessToken(Guid userId, string email, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }

    public Guid? ValidateAccessTokenAndGetUserId(string token)
    {
        var principal = GetPrincipalFromToken(token, validateLifetime: true);
        return ExtractUserId(principal);
    }

    public Guid? GetUserIdFromExpiredToken(string expiredAccessToken)
    {
        var principal = GetPrincipalFromToken(expiredAccessToken, validateLifetime: false);
        return ExtractUserId(principal);
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string token, bool validateLifetime)
    {
        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.SecretKey));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,

            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var securityToken);

            // Extra safety check: make sure the token actually uses the algorithm we expect (HMAC-SHA256),
            // to guard against "alg: none" or algorithm-confusion attacks.
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private static Guid? ExtractUserId(ClaimsPrincipal? principal)
    {
        var userIdClaim = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}