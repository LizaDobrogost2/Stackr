using HomeSavingsTracker.Domain.Enums;
using MediatR;

namespace HomeSavingsTracker.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, CategoryType Type) : IRequest;
