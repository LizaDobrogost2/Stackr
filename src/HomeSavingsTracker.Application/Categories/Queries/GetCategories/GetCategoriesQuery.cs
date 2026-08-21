using MediatR;

namespace HomeSavingsTracker.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
