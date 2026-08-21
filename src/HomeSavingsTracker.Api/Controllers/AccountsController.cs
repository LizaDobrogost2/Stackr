using HomeSavingsTracker.Api.Contracts.Accounts;
using HomeSavingsTracker.Application.Accounts.Commands.CreateAccount;
using HomeSavingsTracker.Application.Accounts.Commands.DeleteAccount;
using HomeSavingsTracker.Application.Accounts.Commands.UpdateAccount;
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateAccountCommand(id, request.Name, request.Type, request.CurrentBalance);
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteAccountCommand(id), cancellationToken);
        return NoContent();
    }
}
