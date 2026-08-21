namespace ContractorPro.Infrastructure.Entities;

public sealed class Contractor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Status { get; set; } = "active";

    public string Timezone { get; set; } = "America/Chicago";

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}
