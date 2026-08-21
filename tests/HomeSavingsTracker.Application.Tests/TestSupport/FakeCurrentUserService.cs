using HomeSavingsTracker.Application.Common.Interfaces;

namespace HomeSavingsTracker.Application.Tests.TestSupport;

public class FakeCurrentUserService(string userId) : ICurrentUserService
{
    public string? UserId { get; } = userId;
}
