using MediatR;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.CreateSavingsGoal;

public record CreateSavingsGoalCommand(
    string Name,
    decimal TargetAmount,
    DateOnly TargetDate,
    Guid AccountId) : IRequest<Guid>;
