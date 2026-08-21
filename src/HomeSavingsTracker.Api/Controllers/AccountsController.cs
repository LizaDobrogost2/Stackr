using HomeSavingsTracker.Api.Contracts.Accounts;
using HomeSavingsTracker.Application.Accounts.Commands.CreateAccount;
using HomeSavingsTracker.Application.Accounts.Queries.GetAccounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts")]
public class AccountsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(request.Name, request.Type, request.CurrentBalance);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await sender.Send(new GetAccountsQuery(), cancellationToken);
        return Ok(accounts);
    }
}
