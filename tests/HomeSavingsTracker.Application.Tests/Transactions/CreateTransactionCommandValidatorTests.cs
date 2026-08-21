using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Transactions.Commands.CreateTransaction;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Transactions;

public class CreateTransactionCommandValidatorTests
{
    private readonly CreateTransactionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new CreateTransactionCommand(
            TransactionType.Expense, 25m, new DateOnly(2026, 8, 1), "Groceries", Guid.NewGuid(), null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_InvalidType_HasError()
    {
        var command = new CreateTransactionCommand(
            (TransactionType)999, 25m, new DateOnly(2026, 8, 1), null, Guid.NewGuid(), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_NonPositiveAmount_HasError(decimal amount)
    {
        var command = new CreateTransactionCommand(
            TransactionType.Expense, amount, new DateOnly(2026, 8, 1), null, Guid.NewGuid(), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Amount);
    }

    [Fact]
    public void Validate_EmptyAccountId_HasError()
    {
        var command = new CreateTransactionCommand(
            TransactionType.Expense, 25m, new DateOnly(2026, 8, 1), null, Guid.Empty, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.AccountId);
    }

    [Fact]
    public void Validate_DescriptionTooLong_HasError()
    {
        var command = new CreateTransactionCommand(
            TransactionType.Expense, 25m, new DateOnly(2026, 8, 1), new string('a', 501), Guid.NewGuid(), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Description);
    }
}
