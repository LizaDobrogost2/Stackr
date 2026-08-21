using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.SavingsGoals.Commands.UpdateSavingsGoal;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.SavingsGoals;

public class UpdateSavingsGoalCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_UpdatesOwnedGoal()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Savings", Type = AccountType.Savings };
        var goal = new SavingsGoal { UserId = UserId, Name = "Old", TargetAmount = 1000m, TargetDate = new DateOnly(2029, 1, 1), AccountId = account.Id };
        dbContext.Accounts.Add(account);
        dbContext.SavingsGoals.Add(goal);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateSavingsGoalCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateSavingsGoalCommand(goal.Id, "New Name", 2000m, new DateOnly(2030, 6, 1));

        await handler.Handle(command, CancellationToken.None);

        var updated = dbContext.SavingsGoals.Single();
        Assert.Equal("New Name", updated.Name);
        Assert.Equal(2000m, updated.TargetAmount);
        Assert.Equal(new DateOnly(2030, 6, 1), updated.TargetDate);
    }

    [Fact]
    public async Task Handle_GoalBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = "someone-else", Name = "Savings", Type = AccountType.Savings };
        var goal = new SavingsGoal { UserId = "someone-else", Name = "Old", TargetAmount = 1000m, TargetDate = new DateOnly(2029, 1, 1), AccountId = account.Id };
        dbContext.Accounts.Add(account);
        dbContext.SavingsGoals.Add(goal);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateSavingsGoalCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateSavingsGoalCommand(goal.Id, "New Name", 2000m, new DateOnly(2030, 6, 1));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
