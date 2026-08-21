using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HomeSavingsTracker.Api.Contracts.Auth;
using HomeSavingsTracker.Api.Contracts.Categories;
using HomeSavingsTracker.Application.Categories.Queries.GetCategories;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.IntegrationTests;

public class CategoriesEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CategoriesEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAndGetAll_ForAuthenticatedUser_ReturnsCreatedCategory()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var createRequest = new CreateCategoryRequest("Groceries", CategoryType.Expense);
        var createResponse = await client.PostAsJsonAsync("/api/categories", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var categoryId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await client.GetAsync("/api/categories");
        getResponse.EnsureSuccessStatusCode();

        var categories = await getResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        var category = Assert.Single(categories!);
        Assert.Equal(categoryId, category.Id);
        Assert.Equal("Groceries", category.Name);
        Assert.Equal(CategoryType.Expense, category.Type);
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
