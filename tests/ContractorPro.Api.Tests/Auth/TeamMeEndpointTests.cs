using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using ContractorPro.Api;
using ContractorPro.Infrastructure;
using ContractorPro.Infrastructure.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Xunit;

namespace ContractorPro.Api.Tests.Auth;

public sealed class TeamMeEndpointTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient? _client;

    private static readonly Guid UserId = Guid.Parse("11223344-1122-1122-1122-112233445566");
    private static readonly Guid SecondaryUserId = Guid.Parse("55667788-5566-5566-5566-556677889900");
    private static readonly Guid ContractorId = Guid.Parse("22334455-2233-2233-2233-223344556677");
    private static readonly Guid TeamMemberId = Guid.Parse("33445566-3344-3344-3344-334455667788");
    private static readonly Guid NonOwnerTeamMemberId = Guid.Parse("44556677-4455-4455-4455-445566778899");

    public TeamMeEndpointTests()
    {
        _factory = new WebApplicationFactory<Program>()
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

                    services.AddDbContext<ContractorProDbContext>(options =>
                        options.UseInMemoryDatabase("TeamMeTests"));
                });
            });
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ContractorProDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        dbContext.Users.Add(new User
        {
            Id = UserId,
            Email = "test@example.com",
            DisplayName = "Test User",
            Status = "active"
        });

        dbContext.Users.Add(new User
        {
            Id = SecondaryUserId,
            Email = "member@example.com",
            DisplayName = "Member User",
            Status = "active"
        });

        dbContext.Contractors.Add(new Contractor
        {
            Id = ContractorId,
            Name = "Test Contractor",
            Status = "active",
            Timezone = "America/Chicago"
        });

        dbContext.TeamMembers.Add(new TeamMember
        {
            Id = TeamMemberId,
            ContractorId = ContractorId,
            UserId = UserId,
            Role = "owner",
            IsOwner = true
        });

        dbContext.TeamMembers.Add(new TeamMember
        {
            Id = NonOwnerTeamMemberId,
            ContractorId = ContractorId,
            UserId = SecondaryUserId,
            Role = "member",
            IsOwner = false
        });

        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task TeamMe_Returns401_WhenAnonymous()
    {
        var response = await _client!.GetAsync("/api/v1/team/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TeamMe_Returns200_WhenAuthenticated()
    {
        using var authFactory = CreateAuthFactory(TestAuthHandler.SchemeName, TestAuthHandler.RegisterScheme);

        using var client = authFactory.CreateClient();
        var response = await client.GetAsync("/api/v1/team/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains(UserId.ToString(), json);
        Assert.Contains(ContractorId.ToString(), json);
        Assert.Contains(TeamMemberId.ToString(), json);
        Assert.Contains("America/Chicago", json);
        Assert.Contains("\"role\":\"owner\"", json);
    }

    [Fact]
    public async Task UpdateCompany_Returns200_AndPersistsValues_WhenOwner()
    {
        using var authFactory = CreateAuthFactory(TestAuthHandler.SchemeName, TestAuthHandler.RegisterScheme);
        using var client = authFactory.CreateClient();

        var payload = JsonSerializer.Serialize(new
        {
            name = "Acme Builders",
            timezone = "America/New_York"
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PutAsync("/api/v1/team/company", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ContractorProDbContext>();
        var contractor = await dbContext.Contractors.SingleAsync(c => c.Id == ContractorId);

        Assert.Equal("Acme Builders", contractor.Name);
        Assert.Equal("America/New_York", contractor.Timezone);
    }

    [Fact]
    public async Task UpdateCompany_Returns403_WhenNonOwner()
    {
        using var authFactory = CreateAuthFactory(NonOwnerAuthHandler.SchemeName, NonOwnerAuthHandler.RegisterScheme);
        using var client = authFactory.CreateClient();

        var payload = JsonSerializer.Serialize(new
        {
            name = "Should Not Persist",
            timezone = "America/Los_Angeles"
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PutAsync("/api/v1/team/company", content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ContractorProDbContext>();
        var contractor = await dbContext.Contractors.SingleAsync(c => c.Id == ContractorId);

        Assert.Equal("Test Contractor", contractor.Name);
        Assert.Equal("America/Chicago", contractor.Timezone);
    }

    [Fact]
    public async Task UpdateCompany_Returns400_WhenTimezoneMissing()
    {
        using var authFactory = CreateAuthFactory(TestAuthHandler.SchemeName, TestAuthHandler.RegisterScheme);
        using var client = authFactory.CreateClient();

        var payload = JsonSerializer.Serialize(new
        {
            name = "Acme Builders",
            timezone = ""
        });

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PutAsync("/api/v1/team/company", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private WebApplicationFactory<Program> CreateAuthFactory(
        string schemeName,
        Action<AuthenticationBuilder> registerScheme)
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var authenticationBuilder = services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = schemeName;
                    options.DefaultChallengeScheme = schemeName;
                });

                registerScheme(authenticationBuilder);
            });
        });
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestAuth";

        public static void RegisterScheme(AuthenticationBuilder builder)
        {
            builder.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(SchemeName, _ => { });
        }

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim("cp:user_id", UserId.ToString()),
                new Claim("cp:contractor_id", ContractorId.ToString()),
                new Claim("cp:team_member_id", TeamMemberId.ToString())
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    private sealed class NonOwnerAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "NonOwnerAuth";

        public static void RegisterScheme(AuthenticationBuilder builder)
        {
            builder.AddScheme<AuthenticationSchemeOptions, NonOwnerAuthHandler>(SchemeName, _ => { });
        }

        public NonOwnerAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim("cp:user_id", SecondaryUserId.ToString()),
                new Claim("cp:contractor_id", ContractorId.ToString()),
                new Claim("cp:team_member_id", NonOwnerTeamMemberId.ToString())
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
