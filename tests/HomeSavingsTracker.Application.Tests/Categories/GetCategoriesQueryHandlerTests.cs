using HomeSavingsTracker.Application.Categories.Queries.GetCategories;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Categories;

public class GetCategoriesQueryHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ReturnsOnlyCategoriesForCurrentUser()
    {
        await using var dbContext = TestDbContext.Create();
        dbContext.Categories.AddRange(
            new Category { UserId = UserId, Name = "Groceries", Type = CategoryType.Expense },
            new Category { UserId = "someone-else", Name = "Salary", Type = CategoryType.Income });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetCategoriesQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        var categories = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        var category = Assert.Single(categories);
        Assert.Equal("Groceries", category.Name);
        Assert.Equal(CategoryType.Expense, category.Type);
    }
}
