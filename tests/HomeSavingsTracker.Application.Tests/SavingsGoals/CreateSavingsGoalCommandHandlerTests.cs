using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.SavingsGoals.Commands.CreateSavingsGoal;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.SavingsGoals;

public class CreateSavingsGoalCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_CreatesSavingsGoalForOwnedAccount()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Savings", Type = AccountType.Savings };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateSavingsGoalCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateSavingsGoalCommand("Home Down Payment", 50000m, new DateOnly(2029, 1, 1), account.Id);

        var goalId = await handler.Handle(command, CancellationToken.None);

        var savedGoal = dbContext.SavingsGoals.Single();
        Assert.Equal(goalId, savedGoal.Id);
        Assert.Equal(UserId, savedGoal.UserId);
        Assert.Equal(50000m, savedGoal.TargetAmount);
        Assert.Equal(account.Id, savedGoal.AccountId);
    }

    [Fact]
    public async Task Handle_AccountBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = "someone-else", Name = "Savings", Type = AccountType.Savings };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateSavingsGoalCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateSavingsGoalCommand("Home Down Payment", 50000m, new DateOnly(2029, 1, 1), account.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
