using MediatR;

namespace HomeSavingsTracker.Application.Budgets.Commands.DeleteBudget;

public record DeleteBudgetCommand(Guid Id) : IRequest;
