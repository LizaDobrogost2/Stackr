using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HomeSavingsTracker.Api.Contracts.Auth;
using HomeSavingsTracker.Api.Contracts.SavingsGoals;
using HomeSavingsTracker.Application.SavingsGoals.Queries.GetSavingsGoalProgress;
using HomeSavingsTracker.Domain.Entities;
using HomeSavingsTracker.Domain.Enums;
using HomeSavingsTracker.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace HomeSavingsTracker.Api.IntegrationTests;

public class SavingsGoalsEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SavingsGoalsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAndGetProgress_ForNewGoalWithContributions_ReturnsExpectedProgress()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        registerResponse.EnsureSuccessStatusCode();
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var userId = await GetUserIdAsync(email);
        var accountId = await SeedAccountWithContributionsAsync(userId);

        var createRequest = new CreateSavingsGoalRequest("Home Down Payment", 20000m, new DateOnly(2029, 1, 1), accountId);
        var createResponse = await client.PostAsJsonAsync("/api/savings-goals", createRequest);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var goalId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var progressResponse = await client.GetAsync($"/api/savings-goals/{goalId}/progress");
        progressResponse.EnsureSuccessStatusCode();

        var progress = await progressResponse.Content.ReadFromJsonAsync<SavingsGoalProgressDto>();
        Assert.NotNull(progress);
        Assert.Equal(goalId, progress!.SavingsGoalId);
        Assert.Equal(2000m, progress.CurrentTotal);
        Assert.Equal(0.1m, progress.PercentComplete);
        Assert.False(progress.IsComplete);
        Assert.NotNull(progress.ProjectedCompletionDate);
    }

    [Fact]
    public async Task GetProgress_WithoutAuthentication_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/savings-goals/{Guid.NewGuid()}/progress");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProgress_ForNonexistentGoal_ReturnsNotFoundProblemDetails()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var response = await client.GetAsync($"/api/savings-goals/{Guid.NewGuid()}/progress");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Update_ForOwnedGoal_ChangesArePersisted()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var userId = await GetUserIdAsync(email);
        var accountId = await SeedAccountWithContributionsAsync(userId);

        var createResponse = await client.PostAsJsonAsync("/api/savings-goals", new CreateSavingsGoalRequest("Home Down Payment", 20000m, new DateOnly(2029, 1, 1), accountId));
        var goalId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var updateResponse = await client.PutAsJsonAsync($"/api/savings-goals/{goalId}", new UpdateSavingsGoalRequest("New Car", 15000m, new DateOnly(2028, 1, 1)));
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var progress = await (await client.GetAsync($"/api/savings-goals/{goalId}/progress")).Content.ReadFromJsonAsync<SavingsGoalProgressDto>();
        Assert.Equal("New Car", progress!.Name);
        Assert.Equal(15000m, progress.TargetAmount);
        Assert.Equal(new DateOnly(2028, 1, 1), progress.TargetDate);
    }

    [Fact]
    public async Task Delete_ForOwnedGoal_RemovesIt()
    {
        var client = _factory.CreateClient();

        var email = $"{Guid.NewGuid():N}@example.com";
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "P@ssw0rd123!"));
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        var userId = await GetUserIdAsync(email);
        var accountId = await SeedAccountWithContributionsAsync(userId);

        var createResponse = await client.PostAsJsonAsync("/api/savings-goals", new CreateSavingsGoalRequest("Home Down Payment", 20000m, new DateOnly(2029, 1, 1), accountId));
        var goalId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var deleteResponse = await client.DeleteAsync($"/api/savings-goals/{goalId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var progressResponse = await client.GetAsync($"/api/savings-goals/{goalId}/progress");
        Assert.Equal(HttpStatusCode.NotFound, progressResponse.StatusCode);
    }

    private Task<string> GetUserIdAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = dbContext.Users.Single(u => u.Email == email);
        return Task.FromResult(user.Id);
    }

    private async Task<Guid> SeedAccountWithContributionsAsync(string userId)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var account = new Account { UserId = userId, Name = "Savings", Type = AccountType.Savings };
        dbContext.Accounts.Add(account);
        dbContext.Transactions.AddRange(
            new Transaction { UserId = userId, AccountId = account.Id, Type = TransactionType.Contribution, Amount = 1000m, Date = new DateOnly(2026, 1, 1) },
            new Transaction { UserId = userId, AccountId = account.Id, Type = TransactionType.Contribution, Amount = 1000m, Date = new DateOnly(2026, 2, 1) });

        await dbContext.SaveChangesAsync();

        return account.Id;
    }
}
