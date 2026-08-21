using HomeSavingsTracker.Application.Accounts.Commands.DeleteAccount;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;
using ValidationException = HomeSavingsTracker.Application.Common.Exceptions.ValidationException;

namespace HomeSavingsTracker.Application.Tests.Accounts;

public class DeleteAccountCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_RemovesOwnedAccount()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteAccountCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await handler.Handle(new DeleteAccountCommand(account.Id), CancellationToken.None);

        Assert.Empty(dbContext.Accounts);
    }

    [Fact]
    public async Task Handle_AccountBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = "someone-else", Name = "Checking", Type = AccountType.Checking };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteAccountCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new DeleteAccountCommand(account.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AccountHasSavingsGoals_ThrowsValidationException()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Savings", Type = AccountType.Savings };
        dbContext.Accounts.Add(account);
        dbContext.SavingsGoals.Add(new SavingsGoal { UserId = UserId, Name = "Trip", TargetAmount = 1000m, TargetDate = new DateOnly(2030, 1, 1), AccountId = account.Id });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteAccountCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(new DeleteAccountCommand(account.Id), CancellationToken.None));
    }
}
