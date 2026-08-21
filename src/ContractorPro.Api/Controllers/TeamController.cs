using ContractorPro.Api.Auth;
using ContractorPro.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContractorPro.Api.Controllers;

[ApiController]
[Authorize(Policy = "TeamMember")]
[Route("api/v1/team")]
public sealed class TeamController : ControllerBase
{
    private readonly ContractorProDbContext _dbContext;

    public TeamController(ContractorProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (!TryGetTeamContext(out var context))
        {
            return Unauthorized(new { error = "unauthorized" });
        }

        var sessionContext = await LoadSessionContextAsync(context, cancellationToken);
        if (sessionContext is null)
        {
            return Unauthorized(new { error = "unauthorized" });
        }

        return Ok(new
        {
            user = new
            {
                id = sessionContext.User.Id,
                displayName = sessionContext.User.DisplayName,
                email = sessionContext.User.Email,
                status = sessionContext.User.Status
            },
            teamMember = new
            {
                id = sessionContext.TeamMember.Id,
                userId = sessionContext.TeamMember.UserId,
                contractorId = sessionContext.TeamMember.ContractorId,
                role = sessionContext.TeamMember.Role,
                isOwner = sessionContext.TeamMember.IsOwner,
                createdAtUtc = sessionContext.TeamMember.CreatedAtUtc,
                updatedAtUtc = sessionContext.TeamMember.UpdatedAtUtc
            },
            contractor = new
            {
                id = sessionContext.Contractor.Id,
                name = sessionContext.Contractor.Name,
                timezone = sessionContext.Contractor.Timezone,
                status = sessionContext.Contractor.Status
            }
        });
    }

    [HttpPut("company")]
    public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetTeamContext(out var context))
        {
            return Unauthorized(new { error = "unauthorized" });
        }

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 120)
        {
            return BadRequest(new { error = "invalid_company_name" });
        }

        if (string.IsNullOrWhiteSpace(request.Timezone) || request.Timezone.Length > 64)
        {
            return BadRequest(new { error = "invalid_company_timezone" });
        }

        var teamMember = await _dbContext.TeamMembers
            .SingleOrDefaultAsync(tm =>
                tm.Id == context.TeamMemberId &&
                tm.ContractorId == context.ContractorId &&
                tm.UserId == context.UserId,
                cancellationToken);

        if (teamMember is null)
        {
            return Unauthorized(new { error = "unauthorized" });
        }

        if (!teamMember.IsOwner)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "forbidden" });
        }

        var contractor = await _dbContext.Contractors
            .SingleOrDefaultAsync(c => c.Id == context.ContractorId, cancellationToken);

        if (contractor is null)
        {
            return Unauthorized(new { error = "unauthorized" });
        }

        contractor.Name = request.Name.Trim();
        contractor.Timezone = request.Timezone.Trim();
        contractor.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            contractor = new
            {
                id = contractor.Id,
                name = contractor.Name,
                timezone = contractor.Timezone,
                status = contractor.Status
            }
        });
    }

    private bool TryGetTeamContext(out TeamRequestContext context)
    {
        context = default;

        if (!Guid.TryParse(User.FindFirst(ContractorProClaimTypes.UserId)?.Value, out var userId) ||
            !Guid.TryParse(User.FindFirst(ContractorProClaimTypes.TeamMemberId)?.Value, out var teamMemberId) ||
            !Guid.TryParse(User.FindFirst(ContractorProClaimTypes.ContractorId)?.Value, out var contractorId))
        {
            return false;
        }

        context = new TeamRequestContext(userId, teamMemberId, contractorId);
        return true;
    }

    private async Task<SessionContext?> LoadSessionContextAsync(TeamRequestContext context, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == context.UserId, cancellationToken);

        var teamMember = await _dbContext.TeamMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(tm =>
                tm.Id == context.TeamMemberId &&
                tm.ContractorId == context.ContractorId &&
                tm.UserId == context.UserId,
                cancellationToken);

        var contractor = await _dbContext.Contractors
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == context.ContractorId, cancellationToken);

        if (user is null || teamMember is null || contractor is null)
        {
            return null;
        }

        return new SessionContext(user, teamMember, contractor);
    }

    public sealed class UpdateCompanyRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Timezone { get; set; } = string.Empty;
    }

    private readonly record struct TeamRequestContext(Guid UserId, Guid TeamMemberId, Guid ContractorId);

    private sealed record SessionContext(
        ContractorPro.Infrastructure.Entities.User User,
        ContractorPro.Infrastructure.Entities.TeamMember TeamMember,
        ContractorPro.Infrastructure.Entities.Contractor Contractor);
}
