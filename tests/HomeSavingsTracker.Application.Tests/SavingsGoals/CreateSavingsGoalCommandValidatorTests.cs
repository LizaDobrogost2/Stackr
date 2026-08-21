using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.SavingsGoals.Commands.CreateSavingsGoal;

namespace HomeSavingsTracker.Application.Tests.SavingsGoals;

public class CreateSavingsGoalCommandValidatorTests
{
    private readonly CreateSavingsGoalCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new CreateSavingsGoalCommand(
            "Down Payment",
            50000m,
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
            Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateSavingsGoalCommand(
            string.Empty,
            50000m,
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
            Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_NonPositiveTargetAmount_HasError(decimal targetAmount)
    {
        var command = new CreateSavingsGoalCommand(
            "Down Payment",
            targetAmount,
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
            Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.TargetAmount);
    }

    [Fact]
    public void Validate_TargetDateInPast_HasError()
    {
        var command = new CreateSavingsGoalCommand(
            "Down Payment",
            50000m,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
            Guid.NewGuid());

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.TargetDate);
    }

    [Fact]
    public void Validate_EmptyAccountId_HasError()
    {
        var command = new CreateSavingsGoalCommand(
            "Down Payment",
            50000m,
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)),
            Guid.Empty);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.AccountId);
    }
}
