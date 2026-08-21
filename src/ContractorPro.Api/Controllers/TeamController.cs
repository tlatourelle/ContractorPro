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
        if (!Guid.TryParse(User.FindFirst(ContractorProClaimTypes.UserId)?.Value, out var userId) ||
            !Guid.TryParse(User.FindFirst(ContractorProClaimTypes.TeamMemberId)?.Value, out var teamMemberId) ||
            !Guid.TryParse(User.FindFirst(ContractorProClaimTypes.ContractorId)?.Value, out var contractorId))
        {
            return Unauthorized(new { error = "unauthorized" });
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var teamMember = await _dbContext.TeamMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(tm => tm.Id == teamMemberId && tm.ContractorId == contractorId && tm.UserId == userId, cancellationToken);

        var contractor = await _dbContext.Contractors
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == contractorId, cancellationToken);

        if (user is null || teamMember is null || contractor is null)
        {
            return Unauthorized(new { error = "unauthorized" });
        }

        return Ok(new
        {
            user = new
            {
                id = user.Id,
                displayName = user.DisplayName,
                email = user.Email
            },
            teamMember = new
            {
                id = teamMember.Id,
                role = teamMember.Role,
                isOwner = teamMember.IsOwner
            },
            contractor = new
            {
                id = contractor.Id,
                name = contractor.Name,
                status = contractor.Status
            }
        });
    }
}
