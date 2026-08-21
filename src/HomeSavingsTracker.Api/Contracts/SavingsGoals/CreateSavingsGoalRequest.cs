namespace HomeSavingsTracker.Api.Contracts.SavingsGoals;

public record CreateSavingsGoalRequest(string Name, decimal TargetAmount, DateOnly TargetDate, Guid AccountId);
