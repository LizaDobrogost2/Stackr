using HomeSavingsTracker.Application.Categories.Commands.CreateCategory;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Categories;

public class CreateCategoryCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_CreatesCategoryForCurrentUser()
    {
        await using var dbContext = TestDbContext.Create();
        var handler = new CreateCategoryCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateCategoryCommand("Groceries", CategoryType.Expense);

        var categoryId = await handler.Handle(command, CancellationToken.None);

        var savedCategory = dbContext.Categories.Single();
        Assert.Equal(categoryId, savedCategory.Id);
        Assert.Equal(UserId, savedCategory.UserId);
        Assert.Equal("Groceries", savedCategory.Name);
        Assert.Equal(CategoryType.Expense, savedCategory.Type);
    }
}
