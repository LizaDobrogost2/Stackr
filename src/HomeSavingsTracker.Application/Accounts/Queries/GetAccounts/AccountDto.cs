using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Accounts.Queries.GetAccounts;

public record AccountDto(
    Guid Id,
    string Name,
    AccountType Type,
    decimal CurrentBalance);
