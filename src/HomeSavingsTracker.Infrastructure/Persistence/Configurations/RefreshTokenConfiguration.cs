using HomeSavingsTracker.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSavingsTracker.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(r => r.Token).IsRequired().HasMaxLength(200);
        builder.Property(r => r.UserId).IsRequired();

        builder.HasIndex(r => r.Token).IsUnique();
        builder.HasIndex(r => r.UserId);
    }
}
