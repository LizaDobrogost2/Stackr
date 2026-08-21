using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<UpdateAccountCommand>
{
    public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == userId, cancellationToken);

        if (account is null)
        {
            throw new NotFoundException(nameof(Account), request.Id);
        }

        account.Name = request.Name;
        account.Type = request.Type;
        account.CurrentBalance = request.CurrentBalance;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
