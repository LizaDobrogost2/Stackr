using MediatR;

namespace HomeSavingsTracker.Application.Budgets.Commands.CreateBudget;

public record CreateBudgetCommand(Guid CategoryId, decimal MonthlyLimit, DateOnly Month) : IRequest<Guid>;
