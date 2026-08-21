using ContractorPro.Api;
using ContractorPro.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ContractorPro.Api.Tests;

/// <summary>
/// Integration tests for health endpoints using WebApplicationFactory.
/// </summary>
public class HealthEndpointTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient? _client;

    public HealthEndpointTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // Override DbContext to use in-memory database
                builder.ConfigureServices(services =>
                {
                    var descriptors = services.Where(d =>
                            d.ServiceType == typeof(DbContextOptions<ContractorProDbContext>) ||
                            d.ServiceType == typeof(DbContextOptions) ||
                            d.ServiceType == typeof(ContractorProDbContext) ||
                            d.ServiceType == typeof(IDbContextOptionsConfiguration<ContractorProDbContext>))
                        .ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ContractorProDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        // Seed test data if needed
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ContractorProDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk_WhenDatabaseAvailable()
    {
        // Arrange
        var request = "/api/v1/health";

        // Act
        var response = await _client!.GetAsync(request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"healthy\"", content);
        Assert.Contains("\"database\":\"healthy\"", content);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsDatabaseUnhealthy_WhenConnectionFails()
    {
        // Arrange
        var request = "/api/v1/health";
        
        // Create a factory with a DbContext configured to use an invalid connection string
        var testFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    var descriptors = services.Where(d =>
                            d.ServiceType == typeof(DbContextOptions<ContractorProDbContext>) ||
                            d.ServiceType == typeof(DbContextOptions) ||
                            d.ServiceType == typeof(ContractorProDbContext) ||
                            d.ServiceType == typeof(IDbContextOptionsConfiguration<ContractorProDbContext>))
                        .ToList();

                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Use a Npgsql database with invalid connection string to simulate failure
                    services.AddDbContext<ContractorProDbContext>(options =>
                        options.UseNpgsql("Host=invalid-host;Port=5432;Database=test;Username=test;Password=test"));
                });
            });

        using var client = testFactory.CreateClient();

        // Act
        var response = await client.GetAsync(request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.ServiceUnavailable, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"unhealthy\"", content);
        Assert.Contains("\"database\":\"unhealthy\"", content);
    }

    [Fact]
    public async Task LiveEndpoint_ReturnsOk_WithoutDatabaseCheck()
    {
        // Arrange
        var request = "/api/v1/health/live";

        // Act
        var response = await _client!.GetAsync(request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"status\":\"alive\"", content);
        Assert.DoesNotContain("database", content);
    }
}
