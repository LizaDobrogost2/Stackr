namespace HomeSavingsTracker.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
