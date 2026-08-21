using HomeSavingsTracker.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Accounts.Queries.GetAccounts;

public class GetAccountsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        return await dbContext.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => new AccountDto(a.Id, a.Name, a.Type, a.CurrentBalance))
            .ToListAsync(cancellationToken);
    }
}
