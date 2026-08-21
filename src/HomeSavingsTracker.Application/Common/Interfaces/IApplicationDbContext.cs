using HomeSavingsTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SavingsGoal> SavingsGoals { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Category> Categories { get; }
    DbSet<Budget> Budgets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
