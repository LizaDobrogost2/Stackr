using MediatR;

namespace HomeSavingsTracker.Application.SavingsGoals.Queries.GetSavingsGoalProgress;

public record GetSavingsGoalProgressQuery(Guid SavingsGoalId) : IRequest<SavingsGoalProgressDto>;
