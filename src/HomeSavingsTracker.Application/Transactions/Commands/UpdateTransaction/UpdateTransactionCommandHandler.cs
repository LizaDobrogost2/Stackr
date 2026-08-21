using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Transactions.Commands.UpdateTransaction;

public class UpdateTransactionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<UpdateTransactionCommand>
{
    public async Task Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == userId, cancellationToken);

        if (transaction is null)
        {
            throw new NotFoundException(nameof(Transaction), request.Id);
        }

        if (request.CategoryId is { } categoryId)
        {
            var categoryExists = await dbContext.Categories
                .AnyAsync(c => c.Id == categoryId && c.UserId == userId, cancellationToken);

            if (!categoryExists)
            {
                throw new NotFoundException(nameof(Category), categoryId);
            }
        }

        transaction.Type = request.Type;
        transaction.Amount = request.Amount;
        transaction.Date = request.Date;
        transaction.Description = request.Description;
        transaction.CategoryId = request.CategoryId;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
