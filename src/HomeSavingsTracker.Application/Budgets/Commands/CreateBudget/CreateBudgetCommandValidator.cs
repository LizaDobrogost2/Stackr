using FluentValidation;

namespace HomeSavingsTracker.Application.Budgets.Commands.CreateBudget;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.MonthlyLimit)
            .GreaterThan(0);
    }
}
