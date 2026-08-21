using HomeSavingsTracker.Domain.Common;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Domain.Entities;

public class Account : BaseEntity
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public AccountType Type { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
