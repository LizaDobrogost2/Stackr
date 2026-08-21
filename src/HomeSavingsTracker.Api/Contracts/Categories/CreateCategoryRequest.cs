using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.Contracts.Categories;

public record CreateCategoryRequest(string Name, CategoryType Type);
