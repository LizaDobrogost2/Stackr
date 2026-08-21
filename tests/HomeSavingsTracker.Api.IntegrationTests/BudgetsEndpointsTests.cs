using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HomeSavingsTracker.Api.Contracts.Auth;
using HomeSavingsTracker.Api.Contracts.Budgets;
using HomeSavingsTracker.Api.Contracts.Categories;
using HomeSavingsTracker.Application.Budgets.Queries.GetBudgets;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.IntegrationTests;

public class BudgetsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BudgetsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAndGetAll_ForAuthenticatedUser_ReturnsCreatedBudget()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var categoryResponse = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest("Groceries", CategoryType.Expense));
        var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();

        var createRequest = new CreateBudgetRequest(categoryId, 400m, new DateOnly(2026, 8, 15));
        var createResponse = await client.PostAsJsonAsync("/api/budgets", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var budgetId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await client.GetAsync("/api/budgets?month=2026-08-01");
        getResponse.EnsureSuccessStatusCode();

        var budgets = await getResponse.Content.ReadFromJsonAsync<List<BudgetDto>>();
        Assert.NotNull(budgets);
        var budget = Assert.Single(budgets!);
        Assert.Equal(budgetId, budget.Id);
        Assert.Equal(categoryId, budget.CategoryId);
        Assert.Equal(400m, budget.MonthlyLimit);
        Assert.Equal(new DateOnly(2026, 8, 1), budget.Month);
    }

    [Fact]
    public async Task Create_DuplicateCategoryAndMonth_ReturnsBadRequestProblemDetails()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var categoryResponse = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest("Groceries", CategoryType.Expense));
        var categoryId = await categoryResponse.Content.ReadFromJsonAsync<Guid>();

        await client.PostAsJsonAsync("/api/budgets", new CreateBudgetRequest(categoryId, 400m, new DateOnly(2026, 8, 1)));
        var duplicateResponse = await client.PostAsJsonAsync("/api/budgets", new CreateBudgetRequest(categoryId, 500m, new DateOnly(2026, 8, 20)));

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
        Assert.Equal("application/problem+json", duplicateResponse.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/budgets");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
