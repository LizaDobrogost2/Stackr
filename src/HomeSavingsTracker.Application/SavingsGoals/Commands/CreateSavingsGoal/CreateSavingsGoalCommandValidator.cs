using FluentValidation;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.CreateSavingsGoal;

public class CreateSavingsGoalCommandValidator : AbstractValidator<CreateSavingsGoalCommand>
{
    public CreateSavingsGoalCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TargetAmount)
            .GreaterThan(0);

        RuleFor(x => x.TargetDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.AccountId)
            .NotEmpty();
    }
}
