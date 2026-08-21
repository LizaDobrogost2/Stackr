using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;
using HomeSavingsTracker.Domain.SavingsProgress;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.SavingsGoals.Queries.GetSavingsGoalProgress;

public class GetSavingsGoalProgressQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<GetSavingsGoalProgressQuery, SavingsGoalProgressDto>
{
    public async Task<SavingsGoalProgressDto> Handle(GetSavingsGoalProgressQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var savingsGoal = await dbContext.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == request.SavingsGoalId && g.UserId == userId, cancellationToken);

        if (savingsGoal is null)
        {
            throw new NotFoundException(nameof(Domain.Entities.SavingsGoal), request.SavingsGoalId);
        }

        var contributions = await dbContext.Transactions
            .Where(t => t.AccountId == savingsGoal.AccountId && t.Type == TransactionType.Contribution)
            .Select(t => new Contribution(t.Date, t.Amount))
            .ToListAsync(cancellationToken);

        var progress = SavingsProgressCalculator.Calculate(
            savingsGoal.TargetAmount,
            contributions,
            DateOnly.FromDateTime(DateTime.UtcNow));

        return new SavingsGoalProgressDto(
            savingsGoal.Id,
            savingsGoal.Name,
            progress.TargetAmount,
            savingsGoal.TargetDate,
            progress.CurrentTotal,
            progress.PercentComplete,
            progress.AverageMonthlyContribution,
            progress.ProjectedCompletionDate,
            progress.IsComplete);
    }
}
