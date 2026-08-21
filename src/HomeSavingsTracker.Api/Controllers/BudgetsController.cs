using HomeSavingsTracker.Api.Contracts.Budgets;
using HomeSavingsTracker.Application.Budgets.Commands.CreateBudget;
using HomeSavingsTracker.Application.Budgets.Commands.DeleteBudget;
using HomeSavingsTracker.Application.Budgets.Commands.UpdateBudget;
using HomeSavingsTracker.Application.Budgets.Queries.GetBudgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

/// <summary>Monthly spending limits per category.</summary>
[ApiController]
[Authorize]
[Route("api/budgets")]
public class BudgetsController(ISender sender) : ControllerBase
{
    /// <summary>Creates a monthly budget for a category. The month is normalized to its first day.</summary>
    /// <param name="request">The category, monthly limit, and month (any day within the month is accepted).</param>
    /// <response code="201">The budget was created; returns its id.</response>
    /// <response code="400">The request failed validation, or a budget for this category and month already exists.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No category with the given id exists for the current user.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Create(CreateBudgetRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateBudgetCommand(request.CategoryId, request.MonthlyLimit, request.Month);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    /// <summary>Lists the current user's budgets, optionally filtered to one month.</summary>
    /// <param name="month">When provided, only the budget(s) for this month are returned; any day within the month matches.</param>
    /// <response code="200">Returns the matching budgets.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<BudgetDto>>> GetAll([FromQuery] DateOnly? month, CancellationToken cancellationToken)
    {
        var budgets = await sender.Send(new GetBudgetsQuery(month), cancellationToken);
        return Ok(budgets);
    }

    /// <summary>Updates an owned budget's monthly limit. The category and month cannot be changed.</summary>
    /// <param name="id">The budget's id.</param>
    /// <param name="request">The updated monthly limit.</param>
    /// <response code="204">The budget was updated.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No budget with this id exists for the current user.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateBudgetRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateBudgetCommand(id, request.MonthlyLimit), cancellationToken);
        return NoContent();
    }

    /// <summary>Deletes an owned budget.</summary>
    /// <param name="id">The budget's id.</param>
    /// <response code="204">The budget was deleted.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No budget with this id exists for the current user.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteBudgetCommand(id), cancellationToken);
        return NoContent();
    }
}
