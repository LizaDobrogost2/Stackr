using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.DeleteSavingsGoal;

public class DeleteSavingsGoalCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<DeleteSavingsGoalCommand>
{
    public async Task Handle(DeleteSavingsGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var savingsGoal = await dbContext.SavingsGoals
            .FirstOrDefaultAsync(g => g.Id == request.Id && g.UserId == userId, cancellationToken);

        if (savingsGoal is null)
        {
            throw new NotFoundException(nameof(SavingsGoal), request.Id);
        }

        dbContext.SavingsGoals.Remove(savingsGoal);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
