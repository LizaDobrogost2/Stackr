using HomeSavingsTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSavingsTracker.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.CurrentBalance).HasPrecision(18, 2);

        builder.HasIndex(a => a.UserId);

        builder.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
