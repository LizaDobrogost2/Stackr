using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Transactions.Commands.UpdateTransaction;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Transactions;

public class UpdateTransactionCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_UpdatesOwnedTransaction()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking };
        var transaction = new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Expense, Amount = 25m, Date = new DateOnly(2026, 8, 1) };
        dbContext.Accounts.Add(account);
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateTransactionCommand(transaction.Id, TransactionType.Contribution, 500m, new DateOnly(2026, 8, 5), "Updated", null);

        await handler.Handle(command, CancellationToken.None);

        var updated = dbContext.Transactions.Single();
        Assert.Equal(TransactionType.Contribution, updated.Type);
        Assert.Equal(500m, updated.Amount);
        Assert.Equal(new DateOnly(2026, 8, 5), updated.Date);
        Assert.Equal("Updated", updated.Description);
    }

    [Fact]
    public async Task Handle_TransactionBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = "someone-else", Name = "Checking", Type = AccountType.Checking };
        var transaction = new Transaction { UserId = "someone-else", AccountId = account.Id, Type = TransactionType.Expense, Amount = 25m, Date = new DateOnly(2026, 8, 1) };
        dbContext.Accounts.Add(account);
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateTransactionCommand(transaction.Id, TransactionType.Expense, 25m, new DateOnly(2026, 8, 1), null, null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CategoryBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking };
        var transaction = new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Expense, Amount = 25m, Date = new DateOnly(2026, 8, 1) };
        var category = new Category { UserId = "someone-else", Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Accounts.Add(account);
        dbContext.Transactions.Add(transaction);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new UpdateTransactionCommand(transaction.Id, TransactionType.Expense, 25m, new DateOnly(2026, 8, 1), null, category.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
