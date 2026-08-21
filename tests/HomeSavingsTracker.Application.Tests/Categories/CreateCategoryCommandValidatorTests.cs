using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Categories.Commands.CreateCategory;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Categories;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new CreateCategoryCommand("Groceries", CategoryType.Expense);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateCategoryCommand(string.Empty, CategoryType.Expense);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_InvalidType_HasError()
    {
        var command = new CreateCategoryCommand("Groceries", (CategoryType)999);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Type);
    }
}
