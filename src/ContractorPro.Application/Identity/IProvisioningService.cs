namespace ContractorPro.Application.Identity;

public interface IProvisioningService
{
    Task<ProvisioningResult> ProvisionOrLoadAsync(ProvisioningRequest request, CancellationToken cancellationToken = default);
}
