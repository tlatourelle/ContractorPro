namespace ContractorPro.Domain;

/// <summary>
/// Platform-wide settings controlled by administrators.
/// </summary>
public class PlatformSettings
{
    public int Id { get; set; }

    /// <summary>
    /// Dashboard polling interval in seconds (default 60).
    /// </summary>
    public int DashboardPollIntervalSeconds { get; set; } = 60;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
