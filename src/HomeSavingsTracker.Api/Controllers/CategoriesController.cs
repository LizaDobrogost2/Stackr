using HomeSavingsTracker.Api.Contracts.Categories;
using HomeSavingsTracker.Application.Categories.Commands.CreateCategory;
using HomeSavingsTracker.Application.Categories.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeSavingsTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.Type);
        var id = await sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await sender.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(categories);
    }
}
