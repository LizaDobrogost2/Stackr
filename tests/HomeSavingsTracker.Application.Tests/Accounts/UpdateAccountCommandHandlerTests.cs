using HomeSavingsTracker.Application.Accounts.Commands.UpdateAccount;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Accounts;

public class UpdateAccountCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_UpdatesOwnedAccount()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking, CurrentBalance = 100m };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateAccountCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateAccountCommand(account.Id, "Renamed", AccountType.Savings, 250m);

        await handler.Handle(command, CancellationToken.None);

        var updated = dbContext.Accounts.Single();
        Assert.Equal("Renamed", updated.Name);
        Assert.Equal(AccountType.Savings, updated.Type);
        Assert.Equal(250m, updated.CurrentBalance);
    }

    [Fact]
    public async Task Handle_AccountBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = "someone-else", Name = "Checking", Type = AccountType.Checking };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateAccountCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateAccountCommand(account.Id, "Renamed", AccountType.Savings, 250m);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
