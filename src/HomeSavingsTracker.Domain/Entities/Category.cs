using HomeSavingsTracker.Domain.Common;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Domain.Entities;

public class Category : BaseEntity
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public CategoryType Type { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
