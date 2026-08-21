using HomeSavingsTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HomeSavingsTracker.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public CustomWebApplicationFactory()
    {
        // AddInfrastructure() reads Jwt/connection-string config eagerly during service
        // registration, before ConfigureWebHost's ConfigureAppConfiguration hook applies.
        // Environment variables are picked up by WebApplication.CreateBuilder itself, so
        // they're visible in time.
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", "unused-in-tests");
        Environment.SetEnvironmentVariable("Jwt__Secret", "dGVzdC1zaWduaW5nLWtleS1mb3ItaW50ZWdyYXRpb24tdGVzdHM=");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "HomeSavingsTracker.Tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "HomeSavingsTracker.Tests");
        Environment.SetEnvironmentVariable("Jwt__AccessTokenExpirationMinutes", "15");
        Environment.SetEnvironmentVariable("Jwt__RefreshTokenExpirationDays", "7");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
