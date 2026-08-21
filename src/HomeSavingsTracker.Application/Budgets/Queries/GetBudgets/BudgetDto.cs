namespace HomeSavingsTracker.Application.Budgets.Queries.GetBudgets;

public record BudgetDto(Guid Id, Guid CategoryId, decimal MonthlyLimit, DateOnly Month);
