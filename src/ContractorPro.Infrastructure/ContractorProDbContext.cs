using ContractorPro.Domain;
using ContractorPro.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractorPro.Infrastructure;

/// <summary>
/// ContractorPro database context.
/// </summary>
public sealed class ContractorProDbContext : DbContext
{
    public ContractorProDbContext(DbContextOptions<ContractorProDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Platform-wide settings (admin-only configuration).
    /// </summary>
    public DbSet<PlatformSettings> PlatformSettings { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Contractor> Contractors { get; set; } = null!;

    public DbSet<TeamMember> TeamMembers { get; set; } = null!;

    public DbSet<AuthIdentity> AuthIdentities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure snake_case naming convention for PostgreSQL
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Convert table name to snake_case
            var tableName = entity.GetTableName();
            if (tableName != null)
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            // Convert all column names to snake_case
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }

        // PlatformSettings table
        modelBuilder.Entity<PlatformSettings>(entity =>
        {
            entity.ToTable("PlatformSettings");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DashboardPollIntervalSeconds)
                .HasDefaultValue(60);

            entity.Property(e => e.CreatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAtUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Email).HasMaxLength(320).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(32).HasDefaultValue("active").IsRequired();
            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Contractor>(entity =>
        {
            entity.ToTable("contractors");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(120).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(32).HasDefaultValue("active").IsRequired();
            entity.Property(e => e.Timezone).HasMaxLength(64).HasDefaultValue("America/Chicago").IsRequired();
            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.ToTable("team_members");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ContractorId, e.UserId }).IsUnique();

            entity.Property(e => e.Role).HasMaxLength(32).HasDefaultValue("owner").IsRequired();
            entity.Property(e => e.IsOwner).HasDefaultValue(true);
            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Contractor)
                .WithMany(e => e.TeamMembers)
                .HasForeignKey(e => e.ContractorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(e => e.TeamMemberships)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuthIdentity>(entity =>
        {
            entity.ToTable("auth_identities");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Provider, e.ProviderSubject }).IsUnique();

            entity.Property(e => e.Provider).HasMaxLength(32).IsRequired();
            entity.Property(e => e.ProviderSubject).HasMaxLength(256).IsRequired();
            entity.Property(e => e.EmailAtProvider).HasMaxLength(320).IsRequired();
            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(e => e.AuthIdentities)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    /// <summary>
    /// Convert PascalCase to snake_case.
    /// </summary>
    private static string ToSnakeCase(string pascalCase)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            pascalCase,
            "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
            "_$1",
            System.Text.RegularExpressions.RegexOptions.None,
            System.TimeSpan.FromMilliseconds(100)).ToLower();
    }
}
