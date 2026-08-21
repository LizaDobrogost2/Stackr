using HomeSavingsTracker.Application.Budgets.Queries.GetBudgets;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Budgets;

public class GetBudgetsQueryHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ReturnsOnlyBudgetsForCurrentUser()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = UserId, Name = "Groceries", Type = CategoryType.Expense };
        var otherCategory = new Category { UserId = "someone-else", Name = "Rent", Type = CategoryType.Expense };
        dbContext.Categories.AddRange(category, otherCategory);
        dbContext.Budgets.AddRange(
            new Budget { UserId = UserId, CategoryId = category.Id, MonthlyLimit = 400m, Month = new DateOnly(2026, 8, 1) },
            new Budget { UserId = "someone-else", CategoryId = otherCategory.Id, MonthlyLimit = 1500m, Month = new DateOnly(2026, 8, 1) });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetBudgetsQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        var budgets = await handler.Handle(new GetBudgetsQuery(null), CancellationToken.None);

        var budget = Assert.Single(budgets);
        Assert.Equal(400m, budget.MonthlyLimit);
        Assert.Equal(category.Id, budget.CategoryId);
    }

    [Fact]
    public async Task Handle_WithMonthFilter_ReturnsOnlyMatchingMonthNormalized()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = UserId, Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        dbContext.Budgets.AddRange(
            new Budget { UserId = UserId, CategoryId = category.Id, MonthlyLimit = 400m, Month = new DateOnly(2026, 8, 1) },
            new Budget { UserId = UserId, CategoryId = category.Id, MonthlyLimit = 450m, Month = new DateOnly(2026, 9, 1) });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetBudgetsQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        var budgets = await handler.Handle(new GetBudgetsQuery(new DateOnly(2026, 8, 20)), CancellationToken.None);

        var budget = Assert.Single(budgets);
        Assert.Equal(400m, budget.MonthlyLimit);
    }
}
