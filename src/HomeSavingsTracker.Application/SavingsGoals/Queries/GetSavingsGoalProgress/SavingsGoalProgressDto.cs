namespace HomeSavingsTracker.Application.SavingsGoals.Queries.GetSavingsGoalProgress;

public record SavingsGoalProgressDto(
    Guid SavingsGoalId,
    string Name,
    decimal TargetAmount,
    DateOnly TargetDate,
    decimal CurrentTotal,
    decimal PercentComplete,
    decimal AverageMonthlyContribution,
    DateOnly? ProjectedCompletionDate,
    bool IsComplete);
