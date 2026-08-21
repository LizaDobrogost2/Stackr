using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Transactions.Commands.DeleteTransaction;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Transactions;

public class DeleteTransactionCommandHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ValidCommand_RemovesOwnedTransaction()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking };
        var transaction = new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Expense, Amount = 25m, Date = new DateOnly(2026, 8, 1) };
        dbContext.Accounts.Add(account);
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new DeleteTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await handler.Handle(new DeleteTransactionCommand(transaction.Id), CancellationToken.None);

        Assert.Empty(dbContext.Transactions);
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

        var handler = new DeleteTransactionCommandHandler(dbContext, new FakeCurrentUserService(UserId));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new DeleteTransactionCommand(transaction.Id), CancellationToken.None));
    }
}
