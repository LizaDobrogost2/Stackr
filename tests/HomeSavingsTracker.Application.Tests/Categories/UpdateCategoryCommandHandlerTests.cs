using HomeSavingsTracker.Application.Categories.Commands.UpdateCategory;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Categories;

public class UpdateCategoryCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_UpdatesOwnedCategory()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = UserId, Name = "Old", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCategoryCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateCategoryCommand(category.Id, "Groceries", CategoryType.Expense);

        await handler.Handle(command, CancellationToken.None);

        var updated = dbContext.Categories.Single();
        Assert.Equal("Groceries", updated.Name);
    }

    [Fact]
    public async Task Handle_CategoryBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = "someone-else", Name = "Old", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCategoryCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateCategoryCommand(category.Id, "Groceries", CategoryType.Expense);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
