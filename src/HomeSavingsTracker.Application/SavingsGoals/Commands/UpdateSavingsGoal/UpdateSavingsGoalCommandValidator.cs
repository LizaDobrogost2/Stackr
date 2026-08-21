using FluentValidation;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.UpdateSavingsGoal;

public class UpdateSavingsGoalCommandValidator : AbstractValidator<UpdateSavingsGoalCommand>
{
    public UpdateSavingsGoalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.TargetAmount)
            .GreaterThan(0);

        RuleFor(x => x.TargetDate)
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
