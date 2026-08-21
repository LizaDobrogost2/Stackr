namespace HomeSavingsTracker.Domain.SavingsProgress;

public readonly record struct Contribution(DateOnly Date, decimal Amount);
