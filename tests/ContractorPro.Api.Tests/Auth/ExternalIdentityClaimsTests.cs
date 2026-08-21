using System.Security.Claims;
using ContractorPro.Api.Auth;
using Xunit;

namespace ContractorPro.Api.Tests.Auth;

public sealed class ExternalIdentityClaimsTests
{
    [Fact]
    public void GetProviderSubject_PrefersNameIdentifier_WhenPresent()
    {
        var principal = CreatePrincipal(
            new Claim(ClaimTypes.NameIdentifier, "name-id-123"),
            new Claim("sub", "sub-456"));

        var result = ExternalIdentityClaims.GetProviderSubject(principal);

        Assert.Equal("name-id-123", result);
    }

    [Fact]
    public void GetProviderSubject_UsesSub_WhenNameIdentifierMissing()
    {
        var principal = CreatePrincipal(new Claim("sub", "sub-789"));

        var result = ExternalIdentityClaims.GetProviderSubject(principal);

        Assert.Equal("sub-789", result);
    }

    [Fact]
    public void GetProviderSubject_ReturnsEmpty_WhenNoRelevantClaims()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Email, "owner@example.com"));

        var result = ExternalIdentityClaims.GetProviderSubject(principal);

        Assert.Equal(string.Empty, result);
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));
    }
}
