using MediatR;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.UpdateSavingsGoal;

public record UpdateSavingsGoalCommand(
    Guid Id,
    string Name,
    decimal TargetAmount,
    DateOnly TargetDate) : IRequest;
