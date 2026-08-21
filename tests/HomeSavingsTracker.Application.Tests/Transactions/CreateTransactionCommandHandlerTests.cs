using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Transactions.Commands.CreateTransaction;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Transactions;

public class CreateTransactionCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_CreatesTransactionForOwnedAccount()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Savings", Type = AccountType.Savings };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateTransactionCommand(
            TransactionType.Contribution, 500m, new DateOnly(2026, 8, 1), "Payday transfer", account.Id, null);

        var transactionId = await handler.Handle(command, CancellationToken.None);

        var savedTransaction = dbContext.Transactions.Single();
        Assert.Equal(transactionId, savedTransaction.Id);
        Assert.Equal(UserId, savedTransaction.UserId);
        Assert.Equal(TransactionType.Contribution, savedTransaction.Type);
        Assert.Equal(500m, savedTransaction.Amount);
        Assert.Equal(account.Id, savedTransaction.AccountId);
    }

    [Fact]
    public async Task Handle_AccountBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = "someone-else", Name = "Savings", Type = AccountType.Savings };
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateTransactionCommand(
            TransactionType.Contribution, 500m, new DateOnly(2026, 8, 1), null, account.Id, null);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CategoryBelongsToAnotherUser_ThrowsNotFound()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking };
        var category = new Category { UserId = "someone-else", Name = "Groceries", Type = CategoryType.Expense };
        dbContext.Accounts.Add(account);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));
        var command = new CreateTransactionCommand(
            TransactionType.Expense, 50m, new DateOnly(2026, 8, 1), null, account.Id, category.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}
