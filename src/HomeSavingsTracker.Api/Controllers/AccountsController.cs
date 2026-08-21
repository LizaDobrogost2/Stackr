using HomeSavingsTracker.Api.Contracts.Accounts;
using HomeSavingsTracker.Application.Accounts.Commands.CreateAccount;
using HomeSavingsTracker.Application.Accounts.Commands.DeleteAccount;
using HomeSavingsTracker.Application.Accounts.Commands.UpdateAccount;
using HomeSavingsTracker.Application.Accounts.Queries.GetAccounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

/// <summary>Financial accounts (checking, savings, cash, investment, credit, other) owned by the current user.</summary>
[ApiController]
[Authorize]
[Route("api/accounts")]
public class AccountsController(ISender sender) : ControllerBase
{
    /// <summary>Creates a new account owned by the current user.</summary>
    /// <param name="request">The account's name, type, and starting balance.</param>
    /// <response code="201">The account was created; returns its id.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> Create(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(request.Name, request.Type, request.CurrentBalance);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    /// <summary>Lists all accounts owned by the current user.</summary>
    /// <response code="200">Returns the current user's accounts.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<AccountDto>>> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await sender.Send(new GetAccountsQuery(), cancellationToken);
        return Ok(accounts);
    }

    /// <summary>Updates an owned account's name, type, and current balance.</summary>
    /// <param name="id">The account's id.</param>
    /// <param name="request">The updated name, type, and current balance.</param>
    /// <response code="204">The account was updated.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No account with this id exists for the current user.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateAccountCommand(id, request.Name, request.Type, request.CurrentBalance);
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>Deletes an owned account. Fails if the account still has savings goals attached.</summary>
    /// <param name="id">The account's id.</param>
    /// <response code="204">The account was deleted.</response>
    /// <response code="400">The account still has savings goals attached; delete those first.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No account with this id exists for the current user.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteAccountCommand(id), cancellationToken);
        return NoContent();
    }
}
