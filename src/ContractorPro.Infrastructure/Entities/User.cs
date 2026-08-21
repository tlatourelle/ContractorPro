namespace ContractorPro.Infrastructure.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Status { get; set; } = "active";

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();

    public ICollection<AuthIdentity> AuthIdentities { get; set; } = new List<AuthIdentity>();
}
