using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.SavingsGoals.Queries.GetSavingsGoalProgress;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.SavingsGoals;

public class GetSavingsGoalProgressQueryHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_GoalWithContributions_ReturnsCalculatedProgress()
    {
        await using var dbContext = TestDbContext.Create();

        var account = new Account { UserId = UserId, Name = "Savings", Type = AccountType.Savings };
        var goal = new SavingsGoal
        {
            UserId = UserId,
            Name = "Home Down Payment",
            TargetAmount = 10000m,
            TargetDate = new DateOnly(2028, 1, 1),
            AccountId = account.Id
        };

        dbContext.Accounts.Add(account);
        dbContext.SavingsGoals.Add(goal);
        dbContext.Transactions.AddRange(
            new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Contribution, Amount = 500m, Date = new DateOnly(2026, 1, 1) },
            new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Contribution, Amount = 500m, Date = new DateOnly(2026, 2, 1) },
            new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Expense, Amount = 50m, Date = new DateOnly(2026, 2, 5) });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetSavingsGoalProgressQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        var progress = await handler.Handle(new GetSavingsGoalProgressQuery(goal.Id), CancellationToken.None);

        Assert.Equal(goal.Id, progress.SavingsGoalId);
        Assert.Equal(1000m, progress.CurrentTotal);
        Assert.Equal(0.1m, progress.PercentComplete);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public async Task Handle_GoalDoesNotExistForUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var handler = new GetSavingsGoalProgressQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetSavingsGoalProgressQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
