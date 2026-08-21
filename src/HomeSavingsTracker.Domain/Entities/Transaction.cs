using HomeSavingsTracker.Domain.Common;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Domain.Entities;

public class Transaction : BaseEntity
{
    public required string UserId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string? Description { get; set; }

    public Guid AccountId { get; set; }
    public Account? Account { get; set; }

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
}
