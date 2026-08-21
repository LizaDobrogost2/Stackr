using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.SavingsGoals.Commands.UpdateSavingsGoal;

namespace HomeSavingsTracker.Application.Tests.SavingsGoals;

public class UpdateSavingsGoalCommandValidatorTests
{
    private readonly UpdateSavingsGoalCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new UpdateSavingsGoalCommand(Guid.NewGuid(), "Down Payment", 50000m, DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyId_HasError()
    {
        var command = new UpdateSavingsGoalCommand(Guid.Empty, "Down Payment", 50000m, DateOnly.FromDateTime(DateTime.UtcNow.AddYears(2)));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}
