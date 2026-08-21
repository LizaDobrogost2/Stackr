using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.Contracts.Categories;

public record UpdateCategoryRequest(string Name, CategoryType Type);
