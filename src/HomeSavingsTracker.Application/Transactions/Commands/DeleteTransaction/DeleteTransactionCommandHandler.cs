using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<DeleteTransactionCommand>
{
    public async Task Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == userId, cancellationToken);

        if (transaction is null)
        {
            throw new NotFoundException(nameof(Transaction), request.Id);
        }

        dbContext.Transactions.Remove(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
