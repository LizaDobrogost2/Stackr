using HomeSavingsTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Budgets.Queries.GetBudgets;

public class GetBudgetsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<GetBudgetsQuery, List<BudgetDto>>
{
    public async Task<List<BudgetDto>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var query = dbContext.Budgets.Where(b => b.UserId == userId);

        if (request.Month is { } month)
        {
            var normalizedMonth = new DateOnly(month.Year, month.Month, 1);
            query = query.Where(b => b.Month == normalizedMonth);
        }

        return await query
            .OrderByDescending(b => b.Month)
            .Select(b => new BudgetDto(b.Id, b.CategoryId, b.MonthlyLimit, b.Month))
            .ToListAsync(cancellationToken);
    }
}
