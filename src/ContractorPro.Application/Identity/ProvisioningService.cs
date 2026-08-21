using ContractorPro.Infrastructure;
using ContractorPro.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractorPro.Application.Identity;

public sealed class ProvisioningService : IProvisioningService
{
    private readonly ContractorProDbContext _dbContext;

    public ProvisioningService(ContractorProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProvisioningResult> ProvisionOrLoadAsync(ProvisioningRequest request, CancellationToken cancellationToken = default)
    {
        var provider = request.Provider.Trim().ToLowerInvariant();
        var providerSubject = request.ProviderSubject.Trim();
        var email = request.Email.Trim().ToLowerInvariant();
        var displayName = request.DisplayName.Trim();

        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerSubject) || string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Provider, provider subject, and email are required for provisioning.");
        }

        var identity = await _dbContext.AuthIdentities
            .Include(ai => ai.User)
            .SingleOrDefaultAsync(
                ai => ai.Provider == provider && ai.ProviderSubject == providerSubject,
                cancellationToken);

        if (identity is not null)
        {
            var existingMembership = await _dbContext.TeamMembers
                .AsNoTracking()
                .SingleOrDefaultAsync(tm => tm.UserId == identity.UserId, cancellationToken);

            if (existingMembership is null)
            {
                throw new InvalidOperationException("Existing identity does not have a team membership.");
            }

            identity.LastLoginAtUtc = DateTime.UtcNow;
            identity.EmailAtProvider = email;
            identity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new ProvisioningResult(identity.UserId, existingMembership.Id, existingMembership.ContractorId, Created: false);
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                DisplayName = TruncateDisplayName(displayName, email),
                Status = "active",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var contractor = new Contractor
            {
                Id = Guid.NewGuid(),
                Name = DeriveCompanyName(displayName, email),
                Status = "active",
                Timezone = "America/Chicago",
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            var teamMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                ContractorId = contractor.Id,
                UserId = user.Id,
                Role = "owner",
                IsOwner = true,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            var authIdentity = new AuthIdentity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Provider = provider,
                ProviderSubject = providerSubject,
                EmailAtProvider = email,
                LastLoginAtUtc = now,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            _dbContext.Contractors.Add(contractor);
            _dbContext.TeamMembers.Add(teamMember);
            _dbContext.AuthIdentities.Add(authIdentity);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ProvisioningResult(user.Id, teamMember.Id, contractor.Id, Created: true);
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);

            var concurrentIdentity = await _dbContext.AuthIdentities
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    ai => ai.Provider == provider && ai.ProviderSubject == providerSubject,
                    cancellationToken);

            if (concurrentIdentity is null)
            {
                throw;
            }

            var concurrentMembership = await _dbContext.TeamMembers
                .AsNoTracking()
                .SingleOrDefaultAsync(tm => tm.UserId == concurrentIdentity.UserId, cancellationToken);

            if (concurrentMembership is null)
            {
                throw;
            }

            return new ProvisioningResult(
                concurrentIdentity.UserId,
                concurrentMembership.Id,
                concurrentMembership.ContractorId,
                Created: false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static string TruncateDisplayName(string displayName, string email)
    {
        var candidate = string.IsNullOrWhiteSpace(displayName) ? email : displayName;
        return candidate.Length <= 200 ? candidate : candidate[..200];
    }

    private static string DeriveCompanyName(string displayName, string email)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            return displayName.Length <= 120 ? displayName : displayName[..120];
        }

        var domain = email.Contains('@') ? email.Split('@', 2)[1] : "contractor";
        var token = domain.Split('.', 2)[0];
        var normalized = string.IsNullOrWhiteSpace(token) ? "contractor" : token;

        return char.ToUpperInvariant(normalized[0]) + normalized[1..] + " Contractor";
    }
}
