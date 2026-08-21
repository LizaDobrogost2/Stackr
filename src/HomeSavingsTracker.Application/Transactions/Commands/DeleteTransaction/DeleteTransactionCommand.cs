using MediatR;

namespace HomeSavingsTracker.Application.Transactions.Commands.DeleteTransaction;

public record DeleteTransactionCommand(Guid Id) : IRequest;
