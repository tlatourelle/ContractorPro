using ContractorPro.Api.Auth;

namespace ContractorPro.Api.Middleware;

public sealed class TeamMemberAuthMiddleware
{
    private readonly RequestDelegate _next;

    public TeamMemberAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/v1/team"))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true ||
            !context.User.HasClaim(c => c.Type == ContractorProClaimTypes.UserId) ||
            !context.User.HasClaim(c => c.Type == ContractorProClaimTypes.TeamMemberId) ||
            !context.User.HasClaim(c => c.Type == ContractorProClaimTypes.ContractorId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "unauthorized" });
            return;
        }

        await _next(context);
    }
}
