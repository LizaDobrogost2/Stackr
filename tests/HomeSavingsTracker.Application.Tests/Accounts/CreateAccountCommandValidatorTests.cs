using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Accounts.Commands.CreateAccount;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Accounts;

public class CreateAccountCommandValidatorTests
{
    private readonly CreateAccountCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new CreateAccountCommand("Checking", AccountType.Checking, 100m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var command = new CreateAccountCommand(string.Empty, AccountType.Checking, 100m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_InvalidType_HasError()
    {
        var command = new CreateAccountCommand("Checking", (AccountType)999, 100m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Type);
    }

    [Fact]
    public void Validate_NegativeCurrentBalance_HasError()
    {
        var command = new CreateAccountCommand("Checking", AccountType.Checking, -1m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CurrentBalance);
    }
}
