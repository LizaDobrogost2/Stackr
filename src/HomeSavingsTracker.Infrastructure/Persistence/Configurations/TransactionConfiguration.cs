using HomeSavingsTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSavingsTracker.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Description).HasMaxLength(500);

        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => new { t.AccountId, t.Type });
    }
}
