using HomeSavingsTracker.Domain.Enums;
using MediatR;

namespace HomeSavingsTracker.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, CategoryType Type) : IRequest<Guid>;
