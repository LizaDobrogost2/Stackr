using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Application.Categories.Queries.GetCategories;

public record CategoryDto(Guid Id, string Name, CategoryType Type);
