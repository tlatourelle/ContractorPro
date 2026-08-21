namespace ContractorPro.Application.Identity;

public sealed record ProvisioningRequest(
    string Provider,
    string ProviderSubject,
    string Email,
    string DisplayName);
