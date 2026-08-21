using System.Security.Claims;
using HomeSavingsTracker.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HomeSavingsTracker.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
