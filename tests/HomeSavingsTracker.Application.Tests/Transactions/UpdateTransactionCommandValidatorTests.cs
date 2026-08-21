using FluentValidation.TestHelper;
using HomeSavingsTracker.Application.Transactions.Commands.UpdateTransaction;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Tests.Transactions;

public class UpdateTransactionCommandValidatorTests
{
    private readonly UpdateTransactionCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var command = new UpdateTransactionCommand(Guid.NewGuid(), TransactionType.Expense, 25m, new DateOnly(2026, 8, 1), "Groceries", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyId_HasError()
    {
        var command = new UpdateTransactionCommand(Guid.Empty, TransactionType.Expense, 25m, new DateOnly(2026, 8, 1), null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_NonPositiveAmount_HasError(decimal amount)
    {
        var command = new UpdateTransactionCommand(Guid.NewGuid(), TransactionType.Expense, amount, new DateOnly(2026, 8, 1), null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Amount);
    }
}
