using System.Security.Claims;
using ContractorPro.Api.Auth;
using ContractorPro.Api.Middleware;
using ContractorPro.Application.Identity;
using ContractorPro.Api;
using ContractorPro.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

// Add DbContext
var connectionString = Environment.GetEnvironmentVariable("ConnectionString") 
    ?? builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not configured and ConnectionString environment variable not set.");
    
builder.Services.AddDbContext<ContractorProDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IProvisioningService, ProvisioningService>();

// Add CORS for Vite dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.Configure<ExternalIdAuthenticationOptions>(
    builder.Configuration.GetSection(ExternalIdAuthenticationOptions.SectionName));
builder.Services.AddSingleton<ExternalAuthRuntimeState>();

var externalIdOptions = builder.Configuration
    .GetSection(ExternalIdAuthenticationOptions.SectionName)
    .Get<ExternalIdAuthenticationOptions>() ?? new ExternalIdAuthenticationOptions();

var authBuilder = builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = externalIdOptions.Enabled
            ? OpenIdConnectDefaults.AuthenticationScheme
            : CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = ".ContractorPro.Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Path = "/";
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return context.Response.WriteAsJsonAsync(new { error = "unauthorized" });
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return context.Response.WriteAsJsonAsync(new { error = "forbidden" });
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };
    });

authBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = Uri.IsWellFormedUriString(externalIdOptions.Authority, UriKind.Absolute)
        ? externalIdOptions.Authority
        : "https://example.invalid/common/v2.0";
    options.ClientId = string.IsNullOrWhiteSpace(externalIdOptions.ClientId)
        ? "contractorpro-dev"
        : externalIdOptions.ClientId;
    options.ClientSecret = externalIdOptions.ClientSecret;
    options.CallbackPath = externalIdOptions.CallbackPath;
    options.ResponseType = "code";
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.SaveTokens = false;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var providerSubject = ExternalIdentityClaims.GetProviderSubject(context.Principal);
            var email = context.Principal?.FindFirstValue(ClaimTypes.Email)
                ?? context.Principal?.FindFirstValue("email")
                ?? string.Empty;
            var displayName = context.Principal?.FindFirstValue(ClaimTypes.Name)
                ?? context.Principal?.Identity?.Name
                ?? email;

            var provisioningService = context.HttpContext.RequestServices.GetRequiredService<IProvisioningService>();
            var result = await provisioningService.ProvisionOrLoadAsync(
                new ProvisioningRequest("google", providerSubject, email, displayName),
                context.HttpContext.RequestAborted);

            var claims = new List<Claim>
            {
                new(ContractorProClaimTypes.UserId, result.UserId.ToString()),
                new(ContractorProClaimTypes.TeamMemberId, result.TeamMemberId.ToString()),
                new(ContractorProClaimTypes.ContractorId, result.ContractorId.ToString()),
                new(ClaimTypes.Name, displayName)
            };

            if (!string.IsNullOrWhiteSpace(email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }

            context.Principal = new ClaimsPrincipal(
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TeamMember", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(ContractorProClaimTypes.UserId);
        policy.RequireClaim(ContractorProClaimTypes.TeamMemberId);
        policy.RequireClaim(ContractorProClaimTypes.ContractorId);
    });

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAssertion(_ => true)
        .Build();
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Apply migrations automatically in development
if (app.Environment.IsDevelopment())
{
}
else
{
    // HTTPS redirect only in production
    app.UseHttpsRedirection();
}
app.UseCors("ViteDev");
app.UseAuthentication();
app.UseMiddleware<TeamMemberAuthMiddleware>();
app.UseAuthorization();
app.MapControllers();

// Health check endpoints
app.MapHealthEndpoints();

// Apply migrations for non-test environments only (InMemory DB doesn't support migrations)
if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ContractorProDbContext>();
    
    // Only migrate if using a real database (not InMemory)
    if (dbContext.Database.IsNpgsql())
    {
        await dbContext.Database.MigrateAsync();
    }
}

app.Run();

/// <summary>
/// Partial Program class for WebApplicationFactory integration testing.
/// </summary>
public partial class Program { }
