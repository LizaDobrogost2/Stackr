using HomeSavingsTracker.Api.Contracts.SavingsGoals;
using HomeSavingsTracker.Application.SavingsGoals.Commands.CreateSavingsGoal;
using HomeSavingsTracker.Application.SavingsGoals.Commands.DeleteSavingsGoal;
using HomeSavingsTracker.Application.SavingsGoals.Commands.UpdateSavingsGoal;
using HomeSavingsTracker.Application.SavingsGoals.Queries.GetSavingsGoalProgress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/savings-goals")]
public class SavingsGoalsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateSavingsGoalRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateSavingsGoalCommand(request.Name, request.TargetAmount, request.TargetDate, request.AccountId);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetProgress), new { id }, id);
    }

    [HttpGet("{id:guid}/progress")]
    public async Task<ActionResult<SavingsGoalProgressDto>> GetProgress(Guid id, CancellationToken cancellationToken)
    {
        var progress = await sender.Send(new GetSavingsGoalProgressQuery(id), cancellationToken);
        return Ok(progress);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSavingsGoalRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSavingsGoalCommand(id, request.Name, request.TargetAmount, request.TargetDate);
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteSavingsGoalCommand(id), cancellationToken);
        return NoContent();
    }
}
