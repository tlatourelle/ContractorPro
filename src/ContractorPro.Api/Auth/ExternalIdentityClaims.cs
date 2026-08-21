using System.Security.Claims;

namespace ContractorPro.Api.Auth;

public static class ExternalIdentityClaims
{
    public static string GetProviderSubject(ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("sub")
            ?? string.Empty;
    }
}
