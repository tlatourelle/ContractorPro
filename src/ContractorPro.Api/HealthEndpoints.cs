using ContractorPro.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContractorPro.Api;

/// <summary>
/// Health check endpoints for probing API and database connectivity.
/// </summary>
public static class HealthEndpoints
{
    /// <summary>
    /// Maps health check endpoints.
    /// </summary>
    public static void MapHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1")
            .WithName("Health");

        group.MapGet("/health", GetHealth)
            .WithName("GetHealth")
            .WithDescription("Full health check including database connectivity.");

        group.MapGet("/health/live", GetLive)
            .WithName("GetLive")
            .WithDescription("Liveness probe — no database check.");
    }

    /// <summary>
    /// Full health check: API + database.
    /// </summary>
    private static async Task<IResult> GetHealth(
        [FromServices] ContractorProDbContext dbContext,
        ILogger<Program> logger)
    {
        try
        {
            // Test database connectivity
            var canConnect = await dbContext.Database.CanConnectAsync();

            if (!canConnect)
            {
                logger.LogWarning("Database health check: unable to connect.");
                return Results.Json(new { status = "unhealthy", database = "unhealthy" }, statusCode: StatusCodes.Status503ServiceUnavailable);
            }

            logger.LogInformation("Health check passed.");
            return Results.Ok(new
            {
                status = "healthy",
                database = "healthy"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Health check failed.");
            return Results.Json(new { status = "unhealthy", database = "unhealthy" }, statusCode: StatusCodes.Status503ServiceUnavailable);
        }
    }

    /// <summary>
    /// Liveness probe: no database dependency.
    /// </summary>
    private static IResult GetLive()
    {
        return Results.Ok(new
        {
            status = "alive"
        });
    }
}
