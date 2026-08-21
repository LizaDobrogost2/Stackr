using MediatR;

namespace HomeSavingsTracker.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest;
