using HomeSavingsTracker.Application.Budgets.Commands.UpdateBudget;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Budgets;

public class UpdateBudgetCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_UpdatesOwnedBudget()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = UserId, Name = "Groceries", Type = CategoryType.Expense };
        var budget = new Budget { UserId = UserId, CategoryId = category.Id, MonthlyLimit = 300m, Month = new DateOnly(2026, 8, 1) };
        dbContext.Categories.Add(category);
        dbContext.Budgets.Add(budget);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateBudgetCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await handler.Handle(new UpdateBudgetCommand(budget.Id, 450m), CancellationToken.None);

        var updated = dbContext.Budgets.Single();
        Assert.Equal(450m, updated.MonthlyLimit);
    }

    [Fact]
    public async Task Handle_BudgetBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = "someone-else", Name = "Groceries", Type = CategoryType.Expense };
        var budget = new Budget { UserId = "someone-else", CategoryId = category.Id, MonthlyLimit = 300m, Month = new DateOnly(2026, 8, 1) };
        dbContext.Categories.Add(category);
        dbContext.Budgets.Add(budget);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateBudgetCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new UpdateBudgetCommand(budget.Id, 450m), CancellationToken.None));
    }
}
