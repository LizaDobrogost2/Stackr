using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.Contracts.Transactions;

public record CreateTransactionRequest(
    TransactionType Type,
    decimal Amount,
    DateOnly Date,
    string? Description,
    Guid AccountId,
    Guid? CategoryId);
