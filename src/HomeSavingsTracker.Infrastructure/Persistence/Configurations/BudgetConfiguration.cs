using HomeSavingsTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSavingsTracker.Infrastructure.Persistence.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.Property(b => b.UserId).IsRequired();
        builder.Property(b => b.MonthlyLimit).HasPrecision(18, 2);

        builder.HasIndex(b => new { b.CategoryId, b.Month }).IsUnique();
    }
}
