using System.Reflection;
using HomeSavingsTracker.Application.Common.Interfaces;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeSavingsTracker.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
