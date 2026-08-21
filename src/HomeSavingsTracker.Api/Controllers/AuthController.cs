using HomeSavingsTracker.Api.Contracts.Auth;
using HomeSavingsTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
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

    [HttpPost("login")]
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

    [HttpPost("refresh")]
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
