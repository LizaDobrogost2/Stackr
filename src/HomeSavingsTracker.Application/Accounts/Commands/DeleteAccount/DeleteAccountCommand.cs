using MediatR;

namespace HomeSavingsTracker.Application.Accounts.Commands.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest;
