using HomeSavingsTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Transactions.Queries.GetTransactions;

public class GetTransactionsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var query = dbContext.Transactions.Where(t => t.UserId == userId);

        if (request.AccountId is { } accountId)
        {
            query = query.Where(t => t.AccountId == accountId);
        }

        return await query
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionDto(t.Id, t.Type, t.Amount, t.Date, t.Description, t.AccountId, t.CategoryId))
            .ToListAsync(cancellationToken);
    }
}
