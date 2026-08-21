using HomeSavingsTracker.Application.Accounts.Commands.CreateAccount;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Accounts;

public class CreateAccountCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_CreatesAccountForCurrentUser()
    {
        await using var dbContext = TestDbContext.Create();
        var handler = new CreateAccountCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateAccountCommand("Emergency Fund", AccountType.Savings, 500m);

        var accountId = await handler.Handle(command, CancellationToken.None);

        var savedAccount = dbContext.Accounts.Single();
        Assert.Equal(accountId, savedAccount.Id);
        Assert.Equal(UserId, savedAccount.UserId);
        Assert.Equal("Emergency Fund", savedAccount.Name);
        Assert.Equal(AccountType.Savings, savedAccount.Type);
        Assert.Equal(500m, savedAccount.CurrentBalance);
    }
}
