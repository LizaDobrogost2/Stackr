using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Budgets.Commands.UpdateBudget;

namespace HomeSavingsTracker.Application.Tests.Budgets;

public class UpdateBudgetCommandValidatorTests
{
    private readonly UpdateBudgetCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new UpdateBudgetCommand(Guid.NewGuid(), 400m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyId_HasError()
    {
        var command = new UpdateBudgetCommand(Guid.Empty, 400m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_NonPositiveMonthlyLimit_HasError(decimal monthlyLimit)
    {
        var command = new UpdateBudgetCommand(Guid.NewGuid(), monthlyLimit);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.MonthlyLimit);
    }
}
