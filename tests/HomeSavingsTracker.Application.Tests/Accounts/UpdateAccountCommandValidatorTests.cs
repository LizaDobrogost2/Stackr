using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Accounts.Commands.UpdateAccount;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Accounts;

public class UpdateAccountCommandValidatorTests
{
    private readonly UpdateAccountCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new UpdateAccountCommand(Guid.NewGuid(), "Checking", AccountType.Checking, 100m);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyId_HasError()
    {
        var command = new UpdateAccountCommand(Guid.Empty, "Checking", AccountType.Checking, 100m);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}
