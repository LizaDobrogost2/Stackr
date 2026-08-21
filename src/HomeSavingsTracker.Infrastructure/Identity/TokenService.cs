using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using HomeSavingsTracker.Infrastructure.Persistence;

namespace HomeSavingsTracker.Infrastructure.Identity;

public class TokenService(ApplicationDbContext dbContext, IOptions<JwtSettings> jwtOptions) : ITokenService
{
    private readonly JwtSettings _settings = jwtOptions.Value;

    public async Task<AuthTokens> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var accessToken = GenerateAccessToken(user, out var expiresAtUtc);
        var refreshToken = GenerateRefreshToken();

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays)
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthTokens(accessToken, expiresAtUtc, refreshToken);
    }

    public async Task<AuthTokens?> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken, cancellationToken);

        if (existing is null || !existing.IsActive)
        {
            return null;
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == existing.UserId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        existing.RevokedAtUtc = DateTime.UtcNow;

        var accessToken = GenerateAccessToken(user, out var expiresAtUtc);
        var newRefreshToken = GenerateRefreshToken();

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays)
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthTokens(accessToken, expiresAtUtc, newRefreshToken);
    }

    private string GenerateAccessToken(ApplicationUser user, out DateTime expiresAtUtc)
    {
        expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
