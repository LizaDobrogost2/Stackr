using HomeSavingsTracker.Api.Contracts.SavingsGoals;
using HomeSavingsTracker.Application.SavingsGoals.Commands.CreateSavingsGoal;
using HomeSavingsTracker.Application.SavingsGoals.Commands.DeleteSavingsGoal;
using HomeSavingsTracker.Application.SavingsGoals.Commands.UpdateSavingsGoal;
using HomeSavingsTracker.Application.SavingsGoals.Queries.GetSavingsGoalProgress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

/// <summary>Savings goals tied to an account, and their progress toward completion.</summary>
[ApiController]
[Authorize]
[Route("api/savings-goals")]
public class SavingsGoalsController(ISender sender) : ControllerBase
{
    /// <summary>Creates a savings goal tied to one of the current user's accounts.</summary>
    /// <param name="request">The goal's name, target amount, target date, and the account it tracks.</param>
    /// <response code="201">The goal was created; returns its id.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No account with the given id exists for the current user.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Create(CreateSavingsGoalRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateSavingsGoalCommand(request.Name, request.TargetAmount, request.TargetDate, request.AccountId);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetProgress), new { id }, id);
    }

    /// <summary>Gets a goal's progress: current total, percent complete, average monthly contribution rate, and a projected completion date, computed from the account's contribution transactions.</summary>
    /// <param name="id">The savings goal's id.</param>
    /// <response code="200">Returns the goal's computed progress.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No savings goal with this id exists for the current user.</response>
    [HttpGet("{id:guid}/progress")]
    [ProducesResponseType(typeof(SavingsGoalProgressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SavingsGoalProgressDto>> GetProgress(Guid id, CancellationToken cancellationToken)
    {
        var progress = await sender.Send(new GetSavingsGoalProgressQuery(id), cancellationToken);
        return Ok(progress);
    }

    /// <summary>Updates an owned goal's name, target amount, and target date. The linked account cannot be changed.</summary>
    /// <param name="id">The savings goal's id.</param>
    /// <param name="request">The updated name, target amount, and target date.</param>
    /// <response code="204">The goal was updated.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No savings goal with this id exists for the current user.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateSavingsGoalRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSavingsGoalCommand(id, request.Name, request.TargetAmount, request.TargetDate);
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>Deletes an owned savings goal.</summary>
    /// <param name="id">The savings goal's id.</param>
    /// <response code="204">The goal was deleted.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No savings goal with this id exists for the current user.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSavingsGoalCommand(id), cancellationToken);
        return NoContent();
    }
}
