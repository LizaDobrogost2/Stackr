using HomeSavingsTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSavingsTracker.Infrastructure.Persistence.Configurations;

public class SavingsGoalConfiguration : IEntityTypeConfiguration<SavingsGoal>
{
    public void Configure(EntityTypeBuilder<SavingsGoal> builder)
    {
        builder.Property(g => g.UserId).IsRequired();
        builder.Property(g => g.Name).IsRequired().HasMaxLength(200);
        builder.Property(g => g.TargetAmount).HasPrecision(18, 2);

        builder.HasIndex(g => g.UserId);

        builder.HasOne(g => g.Account)
            .WithMany()
            .HasForeignKey(g => g.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
