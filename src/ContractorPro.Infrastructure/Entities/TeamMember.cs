namespace ContractorPro.Infrastructure.Entities;

public sealed class TeamMember
{
    public Guid Id { get; set; }

    public Guid ContractorId { get; set; }

    public Guid UserId { get; set; }

    public string Role { get; set; } = "owner";

    public bool IsOwner { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public Contractor Contractor { get; set; } = null!;

    public User User { get; set; } = null!;
}
