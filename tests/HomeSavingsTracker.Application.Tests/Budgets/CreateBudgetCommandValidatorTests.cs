using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Budgets.Commands.CreateBudget;

namespace HomeSavingsTracker.Application.Tests.Budgets;

public class CreateBudgetCommandValidatorTests
{
    private readonly CreateBudgetCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new CreateBudgetCommand(Guid.NewGuid(), 400m, new DateOnly(2026, 8, 1));

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyCategoryId_HasError()
    {
        var command = new CreateBudgetCommand(Guid.Empty, 400m, new DateOnly(2026, 8, 1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CategoryId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_NonPositiveMonthlyLimit_HasError(decimal monthlyLimit)
    {
        var command = new CreateBudgetCommand(Guid.NewGuid(), monthlyLimit, new DateOnly(2026, 8, 1));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.MonthlyLimit);
    }
}
