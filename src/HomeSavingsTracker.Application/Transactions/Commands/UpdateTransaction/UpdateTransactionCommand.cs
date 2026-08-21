using HomeSavingsTracker.Domain.Enums;
using MediatR;

namespace HomeSavingsTracker.Application.Transactions.Commands.UpdateTransaction;

public record UpdateTransactionCommand(
    Guid Id,
    TransactionType Type,
    decimal Amount,
    DateOnly Date,
    string? Description,
    Guid? CategoryId) : IRequest;
