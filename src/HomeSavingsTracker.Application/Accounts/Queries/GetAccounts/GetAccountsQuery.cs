using MediatR;

namespace HomeSavingsTracker.Application.Accounts.Queries.GetAccounts;

public record GetAccountsQuery : IRequest<List<AccountDto>>;
