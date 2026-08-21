using HomeSavingsTracker.Application.Categories.Commands.DeleteCategory;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_RemovesOwnedCategory()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = UserId, Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteCategoryCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await handler.Handle(new DeleteCategoryCommand(category.Id), CancellationToken.None);

        Assert.Empty(dbContext.Categories);
    }

    [Fact]
    public async Task Handle_CategoryBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var category = new Category { UserId = "someone-else", Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteCategoryCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new DeleteCategoryCommand(category.Id), CancellationToken.None));
    }
}
