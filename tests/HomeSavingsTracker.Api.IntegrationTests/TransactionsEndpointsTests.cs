using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HomeSavingsTracker.Api.Contracts.Accounts;
using HomeSavingsTracker.Api.Contracts.Auth;
using HomeSavingsTracker.Api.Contracts.Transactions;
using HomeSavingsTracker.Application.Transactions.Queries.GetTransactions;
using HomeSavingsTracker.Domain.Enums;

namespace HomeSavingsTracker.Api.IntegrationTests;

public class TransactionsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TransactionsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAndGetAll_ForAuthenticatedUser_ReturnsCreatedTransaction()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var accountResponse = await client.PostAsJsonAsync("/api/accounts", new CreateAccountRequest("Checking", AccountType.Checking, 0));
        var accountId = await accountResponse.Content.ReadFromJsonAsync<Guid>();

        var createRequest = new CreateTransactionRequest(
            TransactionType.Contribution, 300m, new DateOnly(2026, 8, 1), "Payday transfer", accountId, null);
        var createResponse = await client.PostAsJsonAsync("/api/transactions", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var transactionId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await client.GetAsync($"/api/transactions?accountId={accountId}");
        getResponse.EnsureSuccessStatusCode();

        var transactions = await getResponse.Content.ReadFromJsonAsync<List<TransactionDto>>();
        Assert.NotNull(transactions);
        var transaction = Assert.Single(transactions!);
        Assert.Equal(transactionId, transaction.Id);
        Assert.Equal(300m, transaction.Amount);
        Assert.Equal(TransactionType.Contribution, transaction.Type);
        Assert.Equal(accountId, transaction.AccountId);
    }

    [Fact]
    public async Task Create_ForAccountBelongingToAnotherUser_ReturnsNotFoundProblemDetails()
    {
        var client = _factory.CreateClient();

        var ownerEmail = $"{Guid.NewGuid():N}@example.com";
        var ownerRegisterResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(ownerEmail, "P@ssw0rd123!"));
        var ownerAuth = await ownerRegisterResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerAuth!.AccessToken);
        var accountResponse = await client.PostAsJsonAsync("/api/accounts", new CreateAccountRequest("Checking", AccountType.Checking, 0));
        var accountId = await accountResponse.Content.ReadFromJsonAsync<Guid>();

        var otherEmail = $"{Guid.NewGuid():N}@example.com";
        var otherRegisterResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(otherEmail, "P@ssw0rd123!"));
        var otherAuth = await otherRegisterResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherAuth!.AccessToken);

        var createRequest = new CreateTransactionRequest(
            TransactionType.Expense, 50m, new DateOnly(2026, 8, 1), null, accountId, null);
        var response = await client.PostAsJsonAsync("/api/transactions", createRequest);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/transactions");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
