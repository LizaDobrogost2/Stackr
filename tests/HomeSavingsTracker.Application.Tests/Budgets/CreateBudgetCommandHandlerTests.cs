using HomeSavingsTracker.Application.Budgets.Commands.CreateBudget;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Budgets;

public class CreateBudgetCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_CreatesBudgetNormalizedToFirstOfMonth()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = UserId, Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateBudgetCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateBudgetCommand(category.Id, 400m, new DateOnly(2026, 8, 15));

        var budgetId = await handler.Handle(command, CancellationToken.None);

        var savedBudget = dbContext.Budgets.Single();
        Assert.Equal(budgetId, savedBudget.Id);
        Assert.Equal(UserId, savedBudget.UserId);
        Assert.Equal(category.Id, savedBudget.CategoryId);
        Assert.Equal(400m, savedBudget.MonthlyLimit);
        Assert.Equal(new DateOnly(2026, 8, 1), savedBudget.Month);
    }

    [Fact]
    public async Task Handle_CategoryBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = "someone-else", Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateBudgetCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateBudgetCommand(category.Id, 400m, new DateOnly(2026, 8, 1));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicateCategoryAndMonth_ThrowsValidationException()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = UserId, Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        dbContext.Budgets.Add(new Budget { UserId = UserId, CategoryId = category.Id, MonthlyLimit = 300m, Month = new DateOnly(2026, 8, 1) });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateBudgetCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateBudgetCommand(category.Id, 400m, new DateOnly(2026, 8, 15));

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
