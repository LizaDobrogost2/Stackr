using FluentValidation;

namespace HomeSavingsTracker.Application.Budgets.Commands.UpdateBudget;

public class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
{
    public UpdateBudgetCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.MonthlyLimit)
            .GreaterThan(0);
    }
}
