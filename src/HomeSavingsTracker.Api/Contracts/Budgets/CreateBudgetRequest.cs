namespace HomeSavingsTracker.Api.Contracts.Budgets;

public record CreateBudgetRequest(Guid CategoryId, decimal MonthlyLimit, DateOnly Month);
