using HomeSavingsTracker.Api.Contracts.Auth;
using HomeSavingsTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

/// <summary>Registration, login, and token refresh.</summary>
[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService) : ControllerBase
{
    /// <summary>Creates a new user account and issues an initial token pair.</summary>
    /// <param name="request">The email and password to register with.</param>
    /// <response code="200">Registration succeeded; returns an access token and refresh token.</response>
    /// <response code="400">The email is already registered, or the password doesn't meet the identity requirements.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        var tokens = await tokenService.IssueTokensAsync(user, cancellationToken);
        return Ok(new AuthResponse(tokens.AccessToken, tokens.AccessTokenExpiresAtUtc, tokens.RefreshToken));
    }

    /// <summary>Authenticates with an email and password and issues a new token pair.</summary>
    /// <param name="request">The account's email and password.</param>
    /// <response code="200">Returns an access token and refresh token.</response>
    /// <response code="401">The email or password is incorrect.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized();
        }

        var tokens = await tokenService.IssueTokensAsync(user, cancellationToken);
        return Ok(new AuthResponse(tokens.AccessToken, tokens.AccessTokenExpiresAtUtc, tokens.RefreshToken));
    }

    /// <summary>Exchanges a valid, unexpired refresh token for a new token pair, revoking the old one.</summary>
    /// <param name="request">The refresh token previously issued by register, login, or refresh.</param>
    /// <response code="200">Returns a new access token and refresh token.</response>
    /// <response code="401">The refresh token is missing, expired, or already revoked.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokens = await tokenService.RefreshTokensAsync(request.RefreshToken, cancellationToken);
        if (tokens is null)
        {
            return Unauthorized();
        }

        return Ok(new AuthResponse(tokens.AccessToken, tokens.AccessTokenExpiresAtUtc, tokens.RefreshToken));
    }
}
