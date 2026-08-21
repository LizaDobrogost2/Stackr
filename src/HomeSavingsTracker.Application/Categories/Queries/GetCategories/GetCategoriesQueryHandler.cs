using HomeSavingsTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        return await dbContext.Categories
            .Where(c => c.UserId == userId)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Type))
            .ToListAsync(cancellationToken);
    }
}
