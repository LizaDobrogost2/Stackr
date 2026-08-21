using HomeSavingsTracker.Application.Accounts.Queries.GetAccounts;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Accounts;

public class GetAccountsQueryHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ReturnsOnlyAccountsForCurrentUser()
    {
        await using var dbContext = TestDbContext.Create();
        dbContext.Accounts.AddRange(
            new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking, CurrentBalance = 100m },
            new Account { UserId = "someone-else", Name = "Savings", Type = AccountType.Savings, CurrentBalance = 200m });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetAccountsQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        var accounts = await handler.Handle(new GetAccountsQuery(), CancellationToken.None);

        var account = Assert.Single(accounts);
        Assert.Equal("Checking", account.Name);
        Assert.Equal(AccountType.Checking, account.Type);
        Assert.Equal(100m, account.CurrentBalance);
    }
}
