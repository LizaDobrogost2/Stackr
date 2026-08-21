using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Application.Tests.TestSupport;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavingsGoal>().Property(g => g.TargetAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Account>().Property(a => a.CurrentBalance).HasPrecision(18, 2);
        modelBuilder.Entity<Transaction>().Property(t => t.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<Budget>().Property(b => b.MonthlyLimit).HasPrecision(18, 2);
    }

    public static TestDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }
}
