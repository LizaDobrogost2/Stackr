using HomeSavingsTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSavingsTracker.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.UserId).IsRequired();

        builder.HasIndex(c => c.UserId);

        builder.HasMany(c => c.Transactions)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Budgets)
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
