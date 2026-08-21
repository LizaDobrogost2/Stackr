using HomeSavingsTracker.Api.Contracts.Transactions;
using HomeSavingsTracker.Application.Transactions.Commands.CreateTransaction;
using HomeSavingsTracker.Application.Transactions.Commands.DeleteTransaction;
using HomeSavingsTracker.Application.Transactions.Commands.UpdateTransaction;
using HomeSavingsTracker.Application.Transactions.Queries.GetTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

/// <summary>Income, expense, and contribution transactions recorded against an account.</summary>
[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionsController(ISender sender) : ControllerBase
{
    /// <summary>Records a transaction against one of the current user's accounts.</summary>
    /// <param name="request">The transaction type (income/expense/contribution), amount, date, optional description, the account it's recorded against, and an optional category.</param>
    /// <response code="201">The transaction was created; returns its id.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">The account, or the category if provided, doesn't exist for the current user.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Create(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTransactionCommand(
            request.Type,
            request.Amount,
            request.Date,
            request.Description,
            request.AccountId,
            request.CategoryId);

        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    /// <summary>Lists the current user's transactions, most recent first, optionally filtered to one account.</summary>
    /// <param name="accountId">When provided, only transactions on this account are returned.</param>
    /// <response code="200">Returns the matching transactions.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TransactionDto>>> GetAll([FromQuery] Guid? accountId, CancellationToken cancellationToken)
    {
        var transactions = await sender.Send(new GetTransactionsQuery(accountId), cancellationToken);
        return Ok(transactions);
    }

    /// <summary>Updates an owned transaction's type, amount, date, description, and category. The account cannot be changed.</summary>
    /// <param name="id">The transaction's id.</param>
    /// <param name="request">The updated type, amount, date, description, and optional category.</param>
    /// <response code="204">The transaction was updated.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No transaction with this id exists for the current user, or the given category doesn't exist for the current user.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTransactionCommand(
            id,
            request.Type,
            request.Amount,
            request.Date,
            request.Description,
            request.CategoryId);

        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>Deletes an owned transaction.</summary>
    /// <param name="id">The transaction's id.</param>
    /// <response code="204">The transaction was deleted.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No transaction with this id exists for the current user.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteTransactionCommand(id), cancellationToken);
        return NoContent();
    }
}
