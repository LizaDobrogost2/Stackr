using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HomeSavingsTracker.Api.Contracts.Accounts;
using HomeSavingsTracker.Api.Contracts.Auth;
using HomeSavingsTracker.Application.Accounts.Queries.GetAccounts;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.IntegrationTests;

public class AccountsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AccountsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAndGetAll_ForAuthenticatedUser_ReturnsCreatedAccount()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var createRequest = new CreateAccountRequest("Emergency Fund", AccountType.Savings, 250m);
        var createResponse = await client.PostAsJsonAsync("/api/accounts", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var accountId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await client.GetAsync("/api/accounts");
        getResponse.EnsureSuccessStatusCode();

        var accounts = await getResponse.Content.ReadFromJsonAsync<List<AccountDto>>();
        Assert.NotNull(accounts);
        var account = Assert.Single(accounts!);
        Assert.Equal(accountId, account.Id);
        Assert.Equal("Emergency Fund", account.Name);
        Assert.Equal(AccountType.Savings, account.Type);
        Assert.Equal(250m, account.CurrentBalance);
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/accounts");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Update_ForOwnedAccount_ChangesArePersisted()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var createResponse = await client.PostAsJsonAsync("/api/accounts", new CreateAccountRequest("Checking", AccountType.Checking, 0));
        var accountId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var updateResponse = await client.PutAsJsonAsync($"/api/accounts/{accountId}", new UpdateAccountRequest("Renamed", AccountType.Savings, 500m));
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var accounts = await (await client.GetAsync("/api/accounts")).Content.ReadFromJsonAsync<List<AccountDto>>();
        var account = Assert.Single(accounts!);
        Assert.Equal("Renamed", account.Name);
        Assert.Equal(AccountType.Savings, account.Type);
        Assert.Equal(500m, account.CurrentBalance);
    }

    [Fact]
    public async Task Delete_ForOwnedAccount_RemovesIt()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var createResponse = await client.PostAsJsonAsync("/api/accounts", new CreateAccountRequest("Checking", AccountType.Checking, 0));
        var accountId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var deleteResponse = await client.DeleteAsync($"/api/accounts/{accountId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var accounts = await (await client.GetAsync("/api/accounts")).Content.ReadFromJsonAsync<List<AccountDto>>();
        Assert.Empty(accounts!);
    }

    [Fact]
    public async Task Delete_ForAccountWithSavingsGoal_ReturnsBadRequestProblemDetails()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var createResponse = await client.PostAsJsonAsync("/api/accounts", new CreateAccountRequest("Savings", AccountType.Savings, 0));
        var accountId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        await client.PostAsJsonAsync("/api/savings-goals", new HomeSavingsTracker.Api.Contracts.SavingsGoals.CreateSavingsGoalRequest(
            "Trip", 1000m, new DateOnly(2030, 1, 1), accountId));

        var deleteResponse = await client.DeleteAsync($"/api/accounts/{accountId}");

        Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
        Assert.Equal("application/problem+json", deleteResponse.Content.Headers.ContentType?.MediaType);
    }
}
