namespace HomeSavingsTracker.Api.Contracts.Auth;

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);

public record RefreshRequest(string RefreshToken);

public record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAtUtc, string RefreshToken);
