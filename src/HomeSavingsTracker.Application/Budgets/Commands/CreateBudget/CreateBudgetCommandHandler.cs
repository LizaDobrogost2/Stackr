using FluentValidation.Results;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Budgets.Commands.CreateBudget;

public class CreateBudgetCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<CreateBudgetCommand, Guid>
{
    public async Task<Guid> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var categoryExists = await dbContext.Categories
            .AnyAsync(c => c.Id == request.CategoryId && c.UserId == userId, cancellationToken);

        if (!categoryExists)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var month = new DateOnly(request.Month.Year, request.Month.Month, 1);

        var budgetExists = await dbContext.Budgets
            .AnyAsync(b => b.CategoryId == request.CategoryId && b.Month == month, cancellationToken);

        if (budgetExists)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(CreateBudgetCommand.Month), "A budget for this category and month already exists.")
            ]);
        }

        var budget = new Budget
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            MonthlyLimit = request.MonthlyLimit,
            Month = month
        };

        dbContext.Budgets.Add(budget);
        await dbContext.SaveChangesAsync(cancellationToken);

        return budget.Id;
    }
}
