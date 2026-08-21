namespace HomeSavingsTracker.Infrastructure.Identity;

public record AuthTokens(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken);

public interface ITokenService
{
    Task<AuthTokens> IssueTokensAsync(ApplicationUser user, CancellationToken cancellationToken);

    Task<AuthTokens?> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken);
}
