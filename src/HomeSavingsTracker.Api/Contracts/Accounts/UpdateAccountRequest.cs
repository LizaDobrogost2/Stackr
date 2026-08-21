using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.Contracts.Accounts;

public record UpdateAccountRequest(string Name, AccountType Type, decimal CurrentBalance);
