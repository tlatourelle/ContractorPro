namespace ContractorPro.Infrastructure.Entities;

public sealed class AuthIdentity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string ProviderSubject { get; set; } = string.Empty;

    public string EmailAtProvider { get; set; } = string.Empty;

    public DateTime LastLoginAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public User User { get; set; } = null!;
}
