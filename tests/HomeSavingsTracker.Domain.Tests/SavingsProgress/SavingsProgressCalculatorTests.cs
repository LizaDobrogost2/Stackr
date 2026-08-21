using HomeSavingsTracker.Domain.SavingsProgress;

namespace HomeSavingsTracker.Domain.Tests.SavingsProgress;

public class SavingsProgressCalculatorTests
{
    [Fact]
    public void Calculate_NoContributions_ReturnsZeroProgress()
    {
        var asOf = new DateOnly(2026, 1, 1);

        var progress = SavingsProgressCalculator.Calculate(1000m, [], asOf);

        Assert.Equal(0m, progress.CurrentTotal);
        Assert.Equal(0m, progress.PercentComplete);
        Assert.Equal(0m, progress.AverageMonthlyContribution);
        Assert.Null(progress.ProjectedCompletionDate);
        Assert.False(progress.IsComplete);
    }

    [Fact]
    public void Calculate_ContributionsMeetOrExceedTarget_MarksComplete()
    {
        Contribution[] contributions =
        [
            new(new DateOnly(2026, 1, 1), 600m),
            new(new DateOnly(2026, 2, 1), 600m)
        ];
        var asOf = new DateOnly(2026, 2, 15);

        var progress = SavingsProgressCalculator.Calculate(1000m, contributions, asOf);

        Assert.Equal(1200m, progress.CurrentTotal);
        Assert.Equal(1.2m, progress.PercentComplete);
        Assert.True(progress.IsComplete);
        Assert.Null(progress.ProjectedCompletionDate);
    }

    [Fact]
    public void Calculate_PartialProgress_ComputesAverageAndProjectedCompletionDate()
    {
        Contribution[] contributions =
        [
            new(new DateOnly(2026, 1, 1), 500m),
            new(new DateOnly(2026, 2, 1), 500m)
        ];
        var asOf = new DateOnly(2026, 2, 1);

        var progress = SavingsProgressCalculator.Calculate(6000m, contributions, asOf);

        Assert.Equal(1000m, progress.CurrentTotal);
        Assert.Equal(1000m / 6000m, progress.PercentComplete);
        Assert.Equal(500m, progress.AverageMonthlyContribution);
        Assert.False(progress.IsComplete);
        Assert.Equal(new DateOnly(2026, 12, 1), progress.ProjectedCompletionDate);
    }

    [Fact]
    public void Calculate_ContributionsWithinSingleMonth_TreatsAverageAsThatMonthsTotal()
    {
        Contribution[] contributions =
        [
            new(new DateOnly(2026, 1, 5), 100m),
            new(new DateOnly(2026, 1, 20), 150m)
        ];
        var asOf = new DateOnly(2026, 1, 25);

        var progress = SavingsProgressCalculator.Calculate(1000m, contributions, asOf);

        Assert.Equal(250m, progress.CurrentTotal);
        Assert.Equal(250m, progress.AverageMonthlyContribution);
        Assert.Equal(new DateOnly(2026, 4, 25), progress.ProjectedCompletionDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Calculate_NonPositiveTargetAmount_Throws(decimal targetAmount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => SavingsProgressCalculator.Calculate(targetAmount, [], new DateOnly(2026, 1, 1)));
    }
}
