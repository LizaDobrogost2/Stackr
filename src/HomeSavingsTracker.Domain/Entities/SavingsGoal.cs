using HomeSavingsTracker.Domain.Common;

namespace HomeSavingsTracker.Domain.Entities;

public class SavingsGoal : BaseEntity
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public decimal TargetAmount { get; set; }
    public DateOnly TargetDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>The account whose Contribution transactions count toward this goal.</summary>
    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
}
