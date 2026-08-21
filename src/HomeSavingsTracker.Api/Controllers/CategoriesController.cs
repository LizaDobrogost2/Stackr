using HomeSavingsTracker.Api.Contracts.Categories;
using HomeSavingsTracker.Application.Categories.Commands.CreateCategory;
using HomeSavingsTracker.Application.Categories.Commands.DeleteCategory;
using HomeSavingsTracker.Application.Categories.Commands.UpdateCategory;
using HomeSavingsTracker.Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

/// <summary>Income and expense categories used to classify transactions and set budgets.</summary>
[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController(ISender sender) : ControllerBase
{
    /// <summary>Creates a new category owned by the current user.</summary>
    /// <param name="request">The category's name and type (income or expense).</param>
    /// <response code="201">The category was created; returns its id.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.Type);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    /// <summary>Lists all categories owned by the current user.</summary>
    /// <response code="200">Returns the current user's categories.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await sender.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(categories);
    }

    /// <summary>Updates an owned category's name and type.</summary>
    /// <param name="id">The category's id.</param>
    /// <param name="request">The updated name and type.</param>
    /// <response code="204">The category was updated.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No category with this id exists for the current user.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(id, request.Name, request.Type);
        await sender.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>Deletes an owned category. Its budgets are deleted with it; transactions referencing it are unlinked, not deleted.</summary>
    /// <param name="id">The category's id.</param>
    /// <response code="204">The category was deleted.</response>
    /// <response code="401">The request is missing a valid access token.</response>
    /// <response code="404">No category with this id exists for the current user.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
