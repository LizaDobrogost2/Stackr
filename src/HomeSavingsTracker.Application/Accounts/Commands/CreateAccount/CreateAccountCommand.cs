using HomeSavingsTracker.Domain.Enums;
using MediatR;

namespace HomeSavingsTracker.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(
    string Name,
    AccountType Type,
    decimal CurrentBalance) : IRequest<Guid>;
