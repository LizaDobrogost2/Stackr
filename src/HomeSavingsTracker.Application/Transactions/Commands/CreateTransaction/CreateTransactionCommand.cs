using HomeSavingsTracker.Domain.Enums;
using MediatR;

namespace HomeSavingsTracker.Application.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
    TransactionType Type,
    decimal Amount,
    DateOnly Date,
    string? Description,
    Guid AccountId,
    Guid? CategoryId) : IRequest<Guid>;
