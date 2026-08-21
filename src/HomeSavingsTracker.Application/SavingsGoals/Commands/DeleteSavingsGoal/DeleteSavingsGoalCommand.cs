using MediatR;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.DeleteSavingsGoal;

public record DeleteSavingsGoalCommand(Guid Id) : IRequest;
