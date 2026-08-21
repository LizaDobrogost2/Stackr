using MediatR;

namespace HomeSavingsTracker.Application.Transactions.Queries.GetTransactions;

public record GetTransactionsQuery(Guid? AccountId) : IRequest<List<TransactionDto>>;
