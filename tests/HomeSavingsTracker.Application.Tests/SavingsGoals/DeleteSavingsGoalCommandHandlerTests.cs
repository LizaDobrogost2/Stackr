using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.SavingsGoals.Commands.DeleteSavingsGoal;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.SavingsGoals;

public class DeleteSavingsGoalCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_RemovesOwnedGoal()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Savings", Type = AccountType.Savings };
        var goal = new SavingsGoal { UserId = UserId, Name = "Trip", TargetAmount = 1000m, TargetDate = new DateOnly(2029, 1, 1), AccountId = account.Id };
        dbContext.Accounts.Add(account);
        dbContext.SavingsGoals.Add(goal);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteSavingsGoalCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await handler.Handle(new DeleteSavingsGoalCommand(goal.Id), CancellationToken.None);

        Assert.Empty(dbContext.SavingsGoals);
    }

    [Fact]
    public async Task Handle_GoalBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = "someone-else", Name = "Savings", Type = AccountType.Savings };
        var goal = new SavingsGoal { UserId = "someone-else", Name = "Trip", TargetAmount = 1000m, TargetDate = new DateOnly(2029, 1, 1), AccountId = account.Id };
        dbContext.Accounts.Add(account);
        dbContext.SavingsGoals.Add(goal);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteSavingsGoalCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new DeleteSavingsGoalCommand(goal.Id), CancellationToken.None));
    }
}
