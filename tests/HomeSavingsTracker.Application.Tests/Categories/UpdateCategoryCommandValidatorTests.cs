using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Categories.Commands.UpdateCategory;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Categories;

public class UpdateCategoryCommandValidatorTests
{
    private readonly UpdateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Groceries", CategoryType.Expense);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyId_HasError()
    {
        var command = new UpdateCategoryCommand(Guid.Empty, "Groceries", CategoryType.Expense);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}
