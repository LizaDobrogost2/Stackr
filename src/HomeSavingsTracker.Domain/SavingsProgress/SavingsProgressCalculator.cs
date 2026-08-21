namespace HomeSavingsTracker.Domain.SavingsProgress;

public static class SavingsProgressCalculator
{
    public static SavingsGoalProgress Calculate(
        decimal targetAmount,
        IReadOnlyCollection<Contribution> contributions,
        DateOnly asOf)
    {
        if (targetAmount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetAmount), "Target amount must be greater than zero.");
        }

        var currentTotal = contributions.Sum(c => c.Amount);
        var percentComplete = currentTotal / targetAmount;
        var isComplete = currentTotal >= targetAmount;
        var averageMonthlyContribution = CalculateAverageMonthlyContribution(contributions, asOf);

        DateOnly? projectedCompletionDate = null;
        if (!isComplete && averageMonthlyContribution > 0)
        {
            var remaining = targetAmount - currentTotal;
            var monthsNeeded = (int)Math.Ceiling(remaining / averageMonthlyContribution);
            projectedCompletionDate = asOf.AddMonths(monthsNeeded);
        }

        return new SavingsGoalProgress(
            targetAmount,
            currentTotal,
            percentComplete,
            averageMonthlyContribution,
            projectedCompletionDate,
            isComplete);
    }

    private static decimal CalculateAverageMonthlyContribution(IReadOnlyCollection<Contribution> contributions, DateOnly asOf)
    {
        if (contributions.Count == 0)
        {
            return 0m;
        }

        var earliestMonth = contributions.Min(c => c.Date);
        var monthsElapsed = MonthsBetweenInclusive(earliestMonth, asOf);
        var total = contributions.Sum(c => c.Amount);

        return total / monthsElapsed;
    }

    private static int MonthsBetweenInclusive(DateOnly start, DateOnly end)
    {
        var months = ((end.Year - start.Year) * 12) + end.Month - start.Month + 1;
        return Math.Max(months, 1);
    }
}
