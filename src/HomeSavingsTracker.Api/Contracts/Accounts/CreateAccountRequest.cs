using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.Contracts.Accounts;

public record CreateAccountRequest(string Name, AccountType Type, decimal CurrentBalance);
