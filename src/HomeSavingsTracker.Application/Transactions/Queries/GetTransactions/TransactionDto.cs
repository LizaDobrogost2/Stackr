using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Transactions.Queries.GetTransactions;

public record TransactionDto(
    Guid Id,
    TransactionType Type,
    decimal Amount,
    DateOnly Date,
    string? Description,
    Guid AccountId,
    Guid? CategoryId);
