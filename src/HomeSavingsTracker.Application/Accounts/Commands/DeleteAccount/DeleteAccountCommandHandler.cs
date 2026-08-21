using FluentValidation.Results;
using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = HomeSavingsTracker.Application.Common.Exceptions.ValidationException;

namespace HomeSavingsTracker.Application.Accounts.Commands.DeleteAccount;

public class DeleteAccountCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var account = await dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.UserId == userId, cancellationToken);

        if (account is null)
        {
            throw new NotFoundException(nameof(Account), request.Id);
        }

        var hasSavingsGoals = await dbContext.SavingsGoals
            .AnyAsync(g => g.AccountId == request.Id, cancellationToken);

        if (hasSavingsGoals)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(request.Id), "Cannot delete an account that has savings goals. Delete the savings goals first.")
            ]);
        }

        dbContext.Accounts.Remove(account);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
