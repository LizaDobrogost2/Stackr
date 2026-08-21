using MediatR;

namespace HomeSavingsTracker.Application.Budgets.Commands.UpdateBudget;

public record UpdateBudgetCommand(Guid Id, decimal MonthlyLimit) : IRequest;
