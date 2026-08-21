using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Budgets.Commands.UpdateBudget;

public class UpdateBudgetCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<UpdateBudgetCommand>
{
    public async Task Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var budget = await dbContext.Budgets
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == userId, cancellationToken);

        if (budget is null)
        {
            throw new NotFoundException(nameof(Budget), request.Id);
        }

        budget.MonthlyLimit = request.MonthlyLimit;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
