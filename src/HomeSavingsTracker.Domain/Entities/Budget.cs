using HomeSavingsTracker.Domain.Common;

namespace HomeSavingsTracker.Domain.Entities;

public class Budget : BaseEntity
{
    public required string UserId { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public decimal MonthlyLimit { get; set; }

    /// <summary>Always the first day of the budgeted month.</summary>
    public DateOnly Month { get; set; }
}
