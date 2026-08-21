using MediatR;

namespace HomeSavingsTracker.Application.Budgets.Queries.GetBudgets;

public record GetBudgetsQuery(DateOnly? Month) : IRequest<List<BudgetDto>>;
