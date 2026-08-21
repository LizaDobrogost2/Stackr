namespace HomeSavingsTracker.Api.Contracts.SavingsGoals;

public record UpdateSavingsGoalRequest(string Name, decimal TargetAmount, DateOnly TargetDate);
