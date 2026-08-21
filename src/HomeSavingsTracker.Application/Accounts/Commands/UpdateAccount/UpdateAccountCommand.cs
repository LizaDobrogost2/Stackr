using HomeSavingsTracker.Domain.Enums;
using MediatR;

namespace HomeSavingsTracker.Application.Accounts.Commands.UpdateAccount;

public record UpdateAccountCommand(
    Guid Id,
    string Name,
    AccountType Type,
    decimal CurrentBalance) : IRequest;
