using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;

namespace HomeSavingsTracker.Application.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            UserId = currentUser.UserId!,
            Name = request.Name,
            Type = request.Type,
            CurrentBalance = request.CurrentBalance
        };

        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
