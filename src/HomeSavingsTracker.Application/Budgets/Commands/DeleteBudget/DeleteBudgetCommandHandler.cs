using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Budgets.Commands.DeleteBudget;

public class DeleteBudgetCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<DeleteBudgetCommand>
{
    public async Task Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var budget = await dbContext.Budgets
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == userId, cancellationToken);

        if (budget is null)
        {
            throw new NotFoundException(nameof(Budget), request.Id);
        }

        dbContext.Budgets.Remove(budget);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
