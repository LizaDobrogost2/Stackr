namespace HomeSavingsTracker.Domain.SavingsProgress;

public sealed record SavingsGoalProgress(
    decimal TargetAmount,
    decimal CurrentTotal,
    decimal PercentComplete,
    decimal AverageMonthlyContribution,
    DateOnly? ProjectedCompletionDate,
    bool IsComplete);
