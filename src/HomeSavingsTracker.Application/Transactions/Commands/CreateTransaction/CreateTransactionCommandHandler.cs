using HomeSavingsTracker.Application.Common.Exceptions;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser) : IRequestHandler<CreateTransactionCommand, Guid>
{
    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId!;

        var accountExists = await dbContext.Accounts
            .AnyAsync(a => a.Id == request.AccountId && a.UserId == userId, cancellationToken);

        if (!accountExists)
        {
            throw new NotFoundException(nameof(Account), request.AccountId);
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

        var transaction = new Transaction
        {
            UserId = userId,
            Type = request.Type,
            Amount = request.Amount,
            Date = request.Date,
            Description = request.Description,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId
        };

        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
