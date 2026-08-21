using HomeSavingsTracker.Api.Contracts.Transactions;
using HomeSavingsTracker.Application.Transactions.Commands.CreateTransaction;
using HomeSavingsTracker.Application.Transactions.Queries.GetTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public class TransactionsController(ISender sender) : ControllerBase
{
    [HttpPost]
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

    [HttpGet]
    public async Task<ActionResult<List<TransactionDto>>> GetAll([FromQuery] Guid? accountId, CancellationToken cancellationToken)
    {
        var transactions = await sender.Send(new GetTransactionsQuery(accountId), cancellationToken);
        return Ok(transactions);
    }
}
