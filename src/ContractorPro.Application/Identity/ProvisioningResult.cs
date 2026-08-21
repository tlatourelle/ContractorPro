namespace ContractorPro.Application.Identity;

public sealed record ProvisioningResult(
    Guid UserId,
    Guid TeamMemberId,
    Guid ContractorId,
    bool Created);
