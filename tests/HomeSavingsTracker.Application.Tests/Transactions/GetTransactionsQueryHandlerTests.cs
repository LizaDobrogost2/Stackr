using HomeSavingsTracker.Application.Transactions.Queries.GetTransactions;
using HomeSavingsTracker.Application.Tests.TestSupport;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Transactions;

public class GetTransactionsQueryHandlerTests
{
    private const string UserId = "user-1";

    [Fact]
    public async Task Handle_ReturnsOnlyTransactionsForCurrentUser()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking };
        var otherAccount = new Account { UserId = "someone-else", Name = "Checking", Type = AccountType.Checking };
        dbContext.Accounts.AddRange(account, otherAccount);
        dbContext.Transactions.AddRange(
            new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Expense, Amount = 25m, Date = new DateOnly(2026, 8, 1) },
            new Transaction { UserId = "someone-else", AccountId = otherAccount.Id, Type = TransactionType.Expense, Amount = 99m, Date = new DateOnly(2026, 8, 1) });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTransactionsQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        var transactions = await handler.Handle(new GetTransactionsQuery(null), CancellationToken.None);

        var transaction = Assert.Single(transactions);
        Assert.Equal(25m, transaction.Amount);
        Assert.Equal(account.Id, transaction.AccountId);
    }

    [Fact]
    public async Task Handle_WithAccountIdFilter_ReturnsOnlyMatchingAccountTransactions()
    {
        await using var dbContext = TestDbContext.Create();
        var account = new Account { UserId = UserId, Name = "Checking", Type = AccountType.Checking };
        var otherAccount = new Account { UserId = UserId, Name = "Savings", Type = AccountType.Savings };
        dbContext.Accounts.AddRange(account, otherAccount);
        dbContext.Transactions.AddRange(
            new Transaction { UserId = UserId, AccountId = account.Id, Type = TransactionType.Expense, Amount = 25m, Date = new DateOnly(2026, 8, 1) },
            new Transaction { UserId = UserId, AccountId = otherAccount.Id, Type = TransactionType.Contribution, Amount = 200m, Date = new DateOnly(2026, 8, 1) });
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTransactionsQueryHandler(dbContext, new FakeCurrentUserService(UserId));

        var transactions = await handler.Handle(new GetTransactionsQuery(otherAccount.Id), CancellationToken.None);

        var transaction = Assert.Single(transactions);
        Assert.Equal(200m, transaction.Amount);
        Assert.Equal(otherAccount.Id, transaction.AccountId);
    }
}
