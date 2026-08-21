using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.Contracts.Transactions;

public record UpdateTransactionRequest(
    TransactionType Type,
    decimal Amount,
    DateOnly Date,
    string? Description,
    Guid? CategoryId);
