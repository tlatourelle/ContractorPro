namespace ContractorPro.Api.Auth;

public sealed class ExternalIdAuthenticationOptions
{
    public const string SectionName = "Authentication:ExternalId";

    public bool Enabled { get; set; }

    public string Authority { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string CallbackPath { get; set; } = "/signin-oidc";
}
