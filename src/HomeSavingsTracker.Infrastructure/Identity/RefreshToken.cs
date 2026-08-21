namespace HomeSavingsTracker.Infrastructure.Identity;

public class RefreshToken
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAtUtc { get; set; }

    public bool IsActive => RevokedAtUtc is null && ExpiresAtUtc > DateTime.UtcNow;
}
