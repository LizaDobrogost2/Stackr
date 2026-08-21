using HomeSavingsTracker.Api.Contracts.Budgets;
using HomeSavingsTracker.Application.Budgets.Commands.CreateBudget;
using HomeSavingsTracker.Application.Budgets.Commands.DeleteBudget;
using HomeSavingsTracker.Application.Budgets.Commands.UpdateBudget;
using HomeSavingsTracker.Application.Budgets.Queries.GetBudgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/budgets")]
public class BudgetsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateBudgetRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBudgetCommand(request.CategoryId, request.MonthlyLimit, request.Month);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<BudgetDto>>> GetAll([FromQuery] DateOnly? month, CancellationToken cancellationToken)
    {
        var budgets = await sender.Send(new GetBudgetsQuery(month), cancellationToken);
        return Ok(budgets);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBudgetRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateBudgetCommand(id, request.MonthlyLimit), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteBudgetCommand(id), cancellationToken);
        return NoContent();
    }
}
