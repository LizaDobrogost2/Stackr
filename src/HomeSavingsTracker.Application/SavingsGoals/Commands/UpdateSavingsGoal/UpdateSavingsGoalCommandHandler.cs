using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.UpdateSavingsGoal;

public class UpdateSavingsGoalCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<UpdateSavingsGoalCommand>
{
    public async Task Handle(UpdateSavingsGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var savingsGoal = await dbContext.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == request.Id && g.UserId == userId, cancellationToken);

        if (savingsGoal is null)
        {
            throw new NotFoundException(nameof(SavingsGoal), request.Id);
        }

        savingsGoal.Name = request.Name;
        savingsGoal.TargetAmount = request.TargetAmount;
        savingsGoal.TargetDate = request.TargetDate;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
