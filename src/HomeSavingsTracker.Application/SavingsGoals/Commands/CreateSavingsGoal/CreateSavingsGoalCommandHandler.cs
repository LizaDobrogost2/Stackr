using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.SavingsGoals.Commands.CreateSavingsGoal;

public class CreateSavingsGoalCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<CreateSavingsGoalCommand, Guid>
{
    public async Task<Guid> Handle(CreateSavingsGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var accountExists = await dbContext.Accounts
            .AnyAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken);

        if (!accountExists)
        {
            throw new NotFoundException(nameof(Account), request.AccountId);
        }

        var savingsGoal = new SavingsGoal
        {
            UserId = userId,
            Name = request.Name,
            TargetAmount = request.TargetAmount,
            TargetDate = request.TargetDate,
            AccountId = request.AccountId
        };

        dbContext.SavingsGoals.Add(savingsGoal);
        await dbContext.SaveChangesAsync(cancellationToken);

        return savingsGoal.Id;
    }
}
